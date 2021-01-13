#include <lib.h>
#include <errno.h>
#include "fsloglib.h"

int startfslog(unsigned short ops2log) {
	if (ops2log < FSOP_NONE || ops2log > FSOP_ALL) {
		errno = EINVAL;
		return -1;
	}

	message m;
	m.m1_i1 = ops2log;

	int r = _syscall(VFS_PROC_NR, STARTFSLOG, &m);

	if (r) {
		errno = r;
		return -1;
	}

	return 0;
}

int stopfslog(unsigned short ops2stoplog) {
	if (ops2stoplog < FSOP_NONE || ops2stoplog > FSOP_ALL) {
		errno = EINVAL;
		return -1;
	}

	message m;
	m.m1_i1 = ops2stoplog;

	int r = _syscall(VFS_PROC_NR, STOPFSLOG, &m);

	if (r) {
		errno = r;
		return -1;
	}

	return 0;
}

int getfsloginf(struct fsloginf* fsloginf) {
	if (!fsloginf) {
		errno = EINVAL;
		return -1;
	}

	message m;
	m.m1_p1 = (char*)fsloginf;

	int r = _syscall(VFS_PROC_NR, GETFSLOGINF, &m);

	if (r) {
		errno = r;
		return -1;
	}

	return 0;
}

int getfslog(struct fsloginf* fsloginf, struct fslogrec fslog[]) {
	if (!fsloginf || !fslog) {
		errno = EINVAL;
		return -1;
	}

	message m;
	m.m1_p1 = (char*)fsloginf;
	m.m1_p2 = (char*)fslog;

	int r = _syscall(VFS_PROC_NR, GETFSLOG, &m);

	if (r) {
		errno = r;
		return -1;
	}

	return 0;
}
