#include "fs.h"
#include <sys/stat.h>
#include <string.h>
#include <minix/com.h>
#include <minix/callnr.h>
#include <minix/vfsif.h>
#include <dirent.h>
#include <assert.h>
#include "file.h"
#include "fproc.h"
#include "path.h"
#include "vnode.h"
#include "param.h"
#include "scratchpad.h"
#include "fslog.h"
#include <unistd.h>

int do_link()
{
	int r = OK;
	struct vnode* vp = NULL, * dirp = NULL;
	struct vmnt* vmp1 = NULL, * vmp2 = NULL;
	char fullpath[PATH_MAX];
	struct lookup resolve;
	vir_bytes vname1, vname2;
	size_t vname1_length, vname2_length;

	vname1 = (vir_bytes)job_m_in.name1;
	vname1_length = job_m_in.name1_length;
	vname2 = (vir_bytes)job_m_in.name2;
	vname2_length = job_m_in.name2_length;

	lookup_init(&resolve, fullpath, PATH_NOFLAGS, &vmp1, &vp);
	resolve.l_vmnt_lock = VMNT_WRITE;
	resolve.l_vnode_lock = VNODE_READ;

	if (fetch_name(vname1, vname1_length, fullpath) != OK) return(err_code);
	if ((vp = eat_path(&resolve, fp)) == NULL) return(err_code);

	lookup_init(&resolve, fullpath, PATH_NOFLAGS, &vmp2, &dirp);
	resolve.l_vmnt_lock = VMNT_READ;
	resolve.l_vnode_lock = VNODE_WRITE;
	if (fetch_name(vname2, vname2_length, fullpath) != OK)
		r = err_code;
	else if ((dirp = last_dir(&resolve, fp)) == NULL)
		r = err_code;

	if (r != OK) {
		unlock_vnode(vp);
		unlock_vmnt(vmp1);
		put_vnode(vp);
		return(r);
	}

	if (vp->v_fs_e != dirp->v_fs_e)
		r = EXDEV;
	else
		r = forbidden(fp, dirp, W_BIT | X_BIT);

	if (r == OK)
		r = req_link(vp->v_fs_e, dirp->v_inode_nr, fullpath,
			vp->v_inode_nr);

	unlock_vnode(vp);
	unlock_vnode(dirp);
	if (vmp2 != NULL) unlock_vmnt(vmp2);
	unlock_vmnt(vmp1);
	put_vnode(vp);
	put_vnode(dirp);
	return(r);
}

int do_unlink()
{
	struct vnode* dirp, * dirp_l, * vp;
	struct vmnt* vmp, * vmp2;
	int r;
	char fullpath[PATH_MAX];
	struct lookup resolve, stickycheck;
	vir_bytes vname;
	size_t vname_length;

	vname = (vir_bytes)job_m_in.name;
	vname_length = job_m_in.name_length;
	if (copy_name(vname_length, fullpath) != OK) {

		if (fetch_name(vname, vname_length, fullpath) != OK) {
			logfserr(FSOP_UNLNK, err_code, fullpath);
			return(err_code);
		}
	}

	lookup_init(&resolve, fullpath, PATH_RET_SYMLINK, &vmp, &dirp_l);
	resolve.l_vmnt_lock = VMNT_WRITE;
	resolve.l_vnode_lock = VNODE_WRITE;

	if ((dirp = last_dir(&resolve, fp)) == NULL) return(err_code);

	if (!S_ISDIR(dirp->v_mode)) {
		unlock_vnode(dirp);
		unlock_vmnt(vmp);
		put_vnode(dirp);
		logfserr(FSOP_UNLNK, ENOTDIR, fullpath);
		return(ENOTDIR);
	}

	if ((r = forbidden(fp, dirp, X_BIT | W_BIT)) != OK) {
		unlock_vnode(dirp);
		unlock_vmnt(vmp);
		put_vnode(dirp);
		logfserr(FSOP_UNLNK, r, fullpath);
		return(r);
	}

	if ((dirp->v_mode & S_ISVTX) == S_ISVTX) {

		lookup_init(&stickycheck, resolve.l_path, PATH_RET_SYMLINK, &vmp2, &vp);
		stickycheck.l_vmnt_lock = VMNT_READ;
		stickycheck.l_vnode_lock = VNODE_READ;
		vp = advance(dirp, &stickycheck, fp);
		assert(vmp2 == NULL);
		if (vp != NULL) {
			if (vp->v_uid != fp->fp_effuid && fp->fp_effuid != SU_UID)
				r = EPERM;
			unlock_vnode(vp);
			put_vnode(vp);
		}
		else {
			r = err_code;
		}
		if (r != OK) {
			unlock_vnode(dirp);
			unlock_vmnt(vmp);
			put_vnode(dirp);
			logfserr(FSOP_UNLNK, r, fullpath);
			return(r);
		}
	}

	upgrade_vmnt_lock(vmp);

	if (job_call_nr == UNLINK)
		r = req_unlink(dirp->v_fs_e, dirp->v_inode_nr, fullpath);
	else
		r = req_rmdir(dirp->v_fs_e, dirp->v_inode_nr, fullpath);

	unlock_vnode(dirp);
	unlock_vmnt(vmp);
	put_vnode(dirp);
	if (r == OK)
		logfsop(FSOP_UNLNK, r, fullpath, -1, 0, -1, -1, -1);
	else
		logfserr(FSOP_UNLNK, r, fullpath);
	return(r);
}

int do_rename()
{
	int r = OK, r1;
	struct vnode* old_dirp = NULL, * new_dirp = NULL, * new_dirp_l = NULL, * vp;
	struct vmnt* oldvmp, * newvmp, * vmp2;
	char old_name[PATH_MAX];
	char fullpath[PATH_MAX];
	struct lookup resolve, stickycheck;
	vir_bytes vname1, vname2;
	size_t vname1_length, vname2_length;

	vname1 = (vir_bytes)job_m_in.name1;
	vname1_length = job_m_in.name1_length;
	vname2 = (vir_bytes)job_m_in.name2;
	vname2_length = job_m_in.name2_length;

	lookup_init(&resolve, fullpath, PATH_RET_SYMLINK, &oldvmp, &old_dirp);

	resolve.l_vmnt_lock = VMNT_WRITE;
	resolve.l_vnode_lock = VNODE_WRITE;

	if (fetch_name(vname1, vname1_length, fullpath) != OK) return(err_code);
	if ((old_dirp = last_dir(&resolve, fp)) == NULL) return(err_code);

	if ((old_dirp->v_mode & S_ISVTX) == S_ISVTX) {
		lookup_init(&stickycheck, resolve.l_path, PATH_RET_SYMLINK, &vmp2, &vp);
		stickycheck.l_vmnt_lock = VMNT_READ;
		stickycheck.l_vnode_lock = VNODE_READ;
		vp = advance(old_dirp, &stickycheck, fp);
		assert(vmp2 == NULL);
		if (vp != NULL) {
			if (vp->v_uid != fp->fp_effuid && fp->fp_effuid != SU_UID)
				r = EPERM;
			unlock_vnode(vp);
			put_vnode(vp);
		}
		else {
			r = err_code;
		}
		if (r != OK) {
			unlock_vnode(old_dirp);
			unlock_vmnt(oldvmp);
			put_vnode(old_dirp);
			return(r);
		}
	}

	if (strlen(fullpath) >= sizeof(old_name)) {
		unlock_vnode(old_dirp);
		unlock_vmnt(oldvmp);
		put_vnode(old_dirp);
		return(ENAMETOOLONG);
	}
	strlcpy(old_name, fullpath, PATH_MAX);

	lookup_init(&resolve, fullpath, PATH_RET_SYMLINK, &newvmp, &new_dirp_l);
	resolve.l_vmnt_lock = VMNT_READ;
	resolve.l_vnode_lock = VNODE_WRITE;
	if (fetch_name(vname2, vname2_length, fullpath) != OK) r = err_code;
	else if ((new_dirp = last_dir(&resolve, fp)) == NULL) r = err_code;

	if (new_dirp == old_dirp) assert(new_dirp_l == NULL);

	if (r != OK) {
		unlock_vnode(old_dirp);
		unlock_vmnt(oldvmp);
		put_vnode(old_dirp);
		return(r);
	}

	if (old_dirp->v_fs_e != new_dirp->v_fs_e) r = EXDEV;


	if ((r1 = forbidden(fp, old_dirp, W_BIT | X_BIT)) != OK ||
		(r1 = forbidden(fp, new_dirp, W_BIT | X_BIT)) != OK) r = r1;

	if (r == OK) {
		upgrade_vmnt_lock(oldvmp);
		r = req_rename(old_dirp->v_fs_e, old_dirp->v_inode_nr, old_name,
			new_dirp->v_inode_nr, fullpath);
	}

	unlock_vnode(old_dirp);
	unlock_vmnt(oldvmp);
	if (new_dirp_l) unlock_vnode(new_dirp_l);
	if (newvmp) unlock_vmnt(newvmp);

	put_vnode(old_dirp);
	put_vnode(new_dirp);

	return(r);
}

int do_truncate()
{
	struct vnode* vp;
	struct vmnt* vmp;
	int r;
	char fullpath[PATH_MAX];
	struct lookup resolve;
	off_t length;
	vir_bytes vname;
	size_t vname_length;

	vname = (vir_bytes)job_m_in.m2_p1;
	vname_length = job_m_in.m2_i1;

	lookup_init(&resolve, fullpath, PATH_NOFLAGS, &vmp, &vp);
	resolve.l_vmnt_lock = VMNT_READ;
	resolve.l_vnode_lock = VNODE_WRITE;

	length = (off_t)job_m_in.flength;
	if (length < 0) return(EINVAL);

	if (fetch_name(vname, vname_length, fullpath) != OK) return(err_code);
	if ((vp = eat_path(&resolve, fp)) == NULL) return(err_code);

	if ((r = forbidden(fp, vp, W_BIT)) == OK) {
		if (S_ISREG(vp->v_mode) && vp->v_size == length)
			r = OK;
		else
			r = truncate_vnode(vp, length);
	}

	unlock_vnode(vp);
	unlock_vmnt(vmp);
	put_vnode(vp);
	return(r);
}

int do_ftruncate()
{

	struct filp* rfilp;
	struct vnode* vp;
	int r;
	off_t length;

	scratch(fp).file.fd_nr = job_m_in.fd;
	length = (off_t)job_m_in.flength;

	if (length < 0) return(EINVAL);

	if ((rfilp = get_filp(scratch(fp).file.fd_nr, VNODE_WRITE)) == NULL)
		return(err_code);

	vp = rfilp->filp_vno;

	if (!(rfilp->filp_mode & W_BIT))
		r = EBADF;
	else if (S_ISREG(vp->v_mode) && vp->v_size == length)
		r = OK;
	else
		r = truncate_vnode(vp, length);

	unlock_filp(rfilp);
	return(r);
}

int truncate_vnode(vp, newsize)
struct vnode* vp;
off_t newsize;
{
	int r;

	assert(tll_locked_by_me(&vp->v_lock));
	if (!S_ISREG(vp->v_mode) && !S_ISFIFO(vp->v_mode)) return(EINVAL);

	if ((r = req_ftrunc(vp->v_fs_e, vp->v_inode_nr, newsize, 0)) == OK)
		vp->v_size = newsize;
	return(r);
}

int do_slink()
{
	int r;
	struct vnode* vp;
	struct vmnt* vmp;
	char fullpath[PATH_MAX];
	struct lookup resolve;
	vir_bytes vname1, vname2;
	size_t vname1_length, vname2_length;

	lookup_init(&resolve, fullpath, PATH_NOFLAGS, &vmp, &vp);
	resolve.l_vmnt_lock = VMNT_WRITE;
	resolve.l_vnode_lock = VNODE_WRITE;

	vname1 = (vir_bytes)job_m_in.name1;
	vname1_length = job_m_in.name1_length;
	vname2 = (vir_bytes)job_m_in.name2;
	vname2_length = job_m_in.name2_length;

	if (vname1_length <= 1) return(ENOENT);
	if (vname1_length >= SYMLINK_MAX) return(ENAMETOOLONG);

	if (fetch_name(vname2, vname2_length, fullpath) != OK) return(err_code);
	if ((vp = last_dir(&resolve, fp)) == NULL) return(err_code);
	if ((r = forbidden(fp, vp, W_BIT | X_BIT)) == OK) {
		r = req_slink(vp->v_fs_e, vp->v_inode_nr, fullpath, who_e, vname1, vname1_length - 1, fp->fp_effuid, fp->fp_effgid);
	}

	unlock_vnode(vp);
	unlock_vmnt(vmp);
	put_vnode(vp);

	return(r);
}

int rdlink_direct(orig_path, link_path, rfp)
char* orig_path;
char link_path[PATH_MAX];
struct fproc* rfp;
{
	int r;
	struct vnode* vp;
	struct vmnt* vmp;
	struct lookup resolve;

	lookup_init(&resolve, link_path, PATH_RET_SYMLINK, &vmp, &vp);
	resolve.l_vmnt_lock = VMNT_READ;
	resolve.l_vnode_lock = VNODE_READ;

	strncpy(link_path, orig_path, PATH_MAX);
	link_path[PATH_MAX - 1] = '\0';
	if ((vp = eat_path(&resolve, rfp)) == NULL) return(err_code);

	if (!S_ISLNK(vp->v_mode))
		r = EINVAL;
	else
		r = req_rdlink(vp->v_fs_e, vp->v_inode_nr, NONE, (vir_bytes)link_path, PATH_MAX - 1, 1);

	if (r > 0) link_path[r] = '\0';

	unlock_vnode(vp);
	unlock_vmnt(vmp);
	put_vnode(vp);

	return r;
}

int do_rdlink()
{
	int r;
	struct vnode* vp;
	struct vmnt* vmp;
	char fullpath[PATH_MAX];
	struct lookup resolve;
	vir_bytes vname;
	size_t vname_length, buf_size;
	vir_bytes buf;

	vname = (vir_bytes)job_m_in.name1;
	vname_length = job_m_in.name1_length;
	buf = (vir_bytes)job_m_in.name2;
	buf_size = (size_t)job_m_in.nbytes;
	if (buf_size > SSIZE_MAX) return(EINVAL);

	lookup_init(&resolve, fullpath, PATH_RET_SYMLINK, &vmp, &vp);
	resolve.l_vmnt_lock = VMNT_READ;
	resolve.l_vnode_lock = VNODE_READ;

	if (fetch_name(vname, vname_length, fullpath) != OK) return(err_code);
	if ((vp = eat_path(&resolve, fp)) == NULL) return(err_code);

	if (!S_ISLNK(vp->v_mode))
		r = EINVAL;
	else
		r = req_rdlink(vp->v_fs_e, vp->v_inode_nr, who_e, buf, buf_size, 0);

	unlock_vnode(vp);
	unlock_vmnt(vmp);
	put_vnode(vp);

	return(r);
}
