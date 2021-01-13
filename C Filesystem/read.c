#include "fs.h"
#include <fcntl.h>
#include <unistd.h>
#include <minix/com.h>
#include <minix/u64.h>
#include "file.h"
#include "fproc.h"
#include "scratchpad.h"
#include "param.h"
#include <dirent.h>
#include <assert.h>
#include <minix/vfsif.h>
#include "vnode.h"
#include "vmnt.h"
#include "fslog.h"

int do_read()
{
	return(do_read_write(READING));
}

void lock_bsf(void)
{
	struct fproc* org_fp;
	struct worker_thread* org_self;

	if (mutex_trylock(&bsf_lock) == 0)
		return;

	org_fp = fp;
	org_self = self;

	if (mutex_lock(&bsf_lock) != 0)
		panic("unable to lock block special file lock");

	fp = org_fp;
	self = org_self;
}

void unlock_bsf(void)
{
	if (mutex_unlock(&bsf_lock) != 0)
		panic("failed to unlock block special file lock");
}

void check_bsf_lock(void)
{
	int r = mutex_trylock(&bsf_lock);

	if (r == -EBUSY)
		panic("bsf_lock locked");
	else if (r != 0)
		panic("bsf_lock weird state");

	unlock_bsf();
}

int do_read_write(rw_flag)
int rw_flag;
{
	struct filp* f;
	tll_access_t locktype;
	int r;

	unsigned short fsopcode = (rw_flag == READING) ? FSOP_READ : FSOP_WRITE;

	scratch(fp).file.fd_nr = job_m_in.fd;
	scratch(fp).io.io_buffer = job_m_in.buffer;
	scratch(fp).io.io_nbytes = (size_t)job_m_in.nbytes;

	locktype = (rw_flag == READING) ? VNODE_READ : VNODE_WRITE;
	if ((f = get_filp(scratch(fp).file.fd_nr, locktype)) == NULL) {
		logfserr_nopath(fsopcode, err_code);
		return (err_code);
	}
	if (((f->filp_mode) & (rw_flag == READING ? R_BIT : W_BIT)) == 0) {
		unlock_filp(f);
		logfserr_nopath(fsopcode, f->filp_mode == FILP_CLOSED ? EIO : EBADF);
		return(f->filp_mode == FILP_CLOSED ? EIO : EBADF);
	}
	if (scratch(fp).io.io_nbytes == 0) {
		unlock_filp(f);
		return(0);
	}

	r = read_write(rw_flag, f, scratch(fp).io.io_buffer, scratch(fp).io.io_nbytes,
		who_e);

	struct vnode* vp = f->filp_vno;
	if (r == OK)
		logfsop_nopath(fsopcode, r, scratch(fp).file.fd_nr, vp->v_mode, vp->v_uid, vp->v_gid, vp->v_size);
	else
		logfserr_nopath(fsopcode, r);

	unlock_filp(f);
	return(r);
}

int read_write(int rw_flag, struct filp* f, char* buf, size_t size, endpoint_t for_e)
{
	register struct vnode* vp;
	u64_t position, res_pos, new_pos;
	unsigned int cum_io, cum_io_incr, res_cum_io;
	int op, oflags, r;

	position = f->filp_pos;
	oflags = f->filp_flags;
	vp = f->filp_vno;
	r = OK;
	cum_io = 0;

	if (size > SSIZE_MAX) return(EINVAL);

	if (S_ISFIFO(vp->v_mode)) {
		if (fp->fp_cum_io_partial != 0) {
			panic("VFS: read_write: fp_cum_io_partial not clear");
		}
		r = rw_pipe(rw_flag, for_e, f, buf, size);
		return(r);
	}

	op = (rw_flag == READING ? VFS_DEV_READ : VFS_DEV_WRITE);

	if (S_ISCHR(vp->v_mode)) {
		dev_t dev;
		int suspend_reopen;

		if (vp->v_sdev == NO_DEV)
			panic("VFS: read_write tries to access char dev NO_DEV");

		suspend_reopen = (f->filp_state & FS_NEEDS_REOPEN);
		dev = (dev_t)vp->v_sdev;

		r = dev_io(op, dev, for_e, buf, position, size, oflags, suspend_reopen);
		if (r >= 0) {
			cum_io = r;
			position = add64ul(position, r);
			r = OK;
		}
	}
	else if (S_ISBLK(vp->v_mode)) {
		if (vp->v_sdev == NO_DEV)
			panic("VFS: read_write tries to access block dev NO_DEV");

		lock_bsf();

		r = req_breadwrite(vp->v_bfs_e, for_e, vp->v_sdev, position, size,
			buf, rw_flag, &res_pos, &res_cum_io);
		if (r == OK) {
			position = res_pos;
			cum_io += res_cum_io;
		}

		unlock_bsf();
	}
	else {
		if (rw_flag == WRITING) {

			if (oflags & O_APPEND) position = cvul64(vp->v_size);
		}

		r = req_readwrite(vp->v_fs_e, vp->v_inode_nr, position, rw_flag, for_e, buf, size, &new_pos, &cum_io_incr);

		if (r >= 0) {
			if (ex64hi(new_pos))
				panic("read_write: bad new pos");

			position = new_pos;
			cum_io += cum_io_incr;
		}
	}

	if (rw_flag == WRITING) {
		if (S_ISREG(vp->v_mode) || S_ISDIR(vp->v_mode)) {
			if (cmp64ul(position, vp->v_size) > 0) {
				if (ex64hi(position) != 0) {
					panic("read_write: file size too big ");
				}
				vp->v_size = ex64lo(position);
			}
		}
	}

	f->filp_pos = position;

	if (r == OK) return(cum_io);
	return(r);
}

int do_getdents()
{
	int r = OK;
	u64_t new_pos;
	register struct filp* rfilp;

	scratch(fp).file.fd_nr = job_m_in.fd;
	scratch(fp).io.io_buffer = job_m_in.buffer;
	scratch(fp).io.io_nbytes = (size_t)job_m_in.nbytes;

	if ((rfilp = get_filp(scratch(fp).file.fd_nr, VNODE_READ)) == NULL)
		return(err_code);

	if (!(rfilp->filp_mode & R_BIT))
		r = EBADF;
	else if (!S_ISDIR(rfilp->filp_vno->v_mode))
		r = EBADF;

	if (r == OK) {
		if (ex64hi(rfilp->filp_pos) != 0)
			panic("do_getdents: can't handle large offsets");

		r = req_getdents(rfilp->filp_vno->v_fs_e, rfilp->filp_vno->v_inode_nr,
			rfilp->filp_pos, scratch(fp).io.io_buffer,
			scratch(fp).io.io_nbytes, &new_pos, 0);

		if (r > 0) rfilp->filp_pos = new_pos;
	}

	unlock_filp(rfilp);
	return(r);
}

int rw_pipe(rw_flag, usr_e, f, buf, req_size)
int rw_flag;
endpoint_t usr_e;
struct filp* f;
char* buf;
size_t req_size;
{
	int r, oflags, partial_pipe = 0;
	size_t size, cum_io, cum_io_incr;
	struct vnode* vp;
	u64_t  position, new_pos;

	assert(tll_locked_by_me(&f->filp_vno->v_lock));
	assert(mutex_trylock(&f->filp_lock) == -EDEADLK);

	oflags = f->filp_flags;
	vp = f->filp_vno;
	position = cvu64(0);


	cum_io = fp->fp_cum_io_partial;

	r = pipe_check(vp, rw_flag, oflags, req_size, 0);
	if (r <= 0) {
		if (r == SUSPEND) pipe_suspend(f, buf, req_size);
		return(r);
	}

	size = r;
	if (size < req_size) partial_pipe = 1;


	if (rw_flag == READING && size > vp->v_size) {
		size = vp->v_size;
	}

	if (vp->v_mapfs_e == 0)
		panic("unmapped pipe");

	r = req_readwrite(vp->v_mapfs_e, vp->v_mapinode_nr, position, rw_flag, usr_e,
		buf, size, &new_pos, &cum_io_incr);

	if (r != OK) {
		return(r);
	}

	if (ex64hi(new_pos))
		panic("rw_pipe: bad new pos");

	cum_io += cum_io_incr;
	buf += cum_io_incr;
	req_size -= cum_io_incr;

	vp->v_size = ex64lo(new_pos);

	if (partial_pipe) {
		if (!(oflags & O_NONBLOCK)) {
			fp->fp_cum_io_partial = cum_io;
			pipe_suspend(f, buf, req_size);
			return(SUSPEND);
		}
	}

	fp->fp_cum_io_partial = 0;

	return(cum_io);
}
