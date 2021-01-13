#include <string.h>
#include <unistd.h>
#include <time.h>
#include <stdio.h>
#include "fs.h"
#include "fproc.h"
#include "fslog.h"

#define INVALID_ARG -22     

#define EVENTS_FD_NR 3 

static char* EVENTS_PATH = "events";

static struct fsloginf fsloginf = { 0, 0, FSOP_NONE };

static struct fslogrec fslog[NR_FSLOGREC];

int do_startfslog() {
	int ops2log = m_in.m1_i1;

	if (ops2log < FSOP_NONE || ops2log > FSOP_ALL) {
		return INVALID_ARG;
	}

	fsloginf.start = 0;
	fsloginf.len = 0;
	fsloginf.ops2log |= ops2log;

	return OK;
}

int do_stopfslog() {
	int ops2stoplog = m_in.m1_i1;

	if (ops2stoplog < FSOP_NONE || ops2stoplog > FSOP_ALL) {
		return INVALID_ARG;
	}

	fsloginf.ops2log &= ~ops2stoplog;

	return OK;
}

int do_getfsloginf() {
	vir_bytes dst_vir = (vir_bytes)m_in.m1_p1;

	if (!dst_vir) return INVALID_ARG;

	return sys_vircopy(SELF, (vir_bytes)&fsloginf, who_e, dst_vir, sizeof(fsloginf));
}

int do_getfslog() {
	printf("entering do_getfslog\n");
	vir_bytes dst1_vir = (vir_bytes)m_in.m1_p1;
	vir_bytes dst2_vir = (vir_bytes)m_in.m1_p2;

	if (!dst1_vir || !dst2_vir) return INVALID_ARG;

	int r = sys_vircopy(SELF, (vir_bytes)&fsloginf, who_e, dst1_vir, sizeof(fsloginf));

	if (r) return r;

	phys_bytes fslogrec_size = sizeof(struct fslogrec) * NR_FSLOGREC;

	return sys_vircopy(SELF, (vir_bytes)&fslog, who_e, dst2_vir, fslogrec_size);
}

void logfserr(int opcode, int result, char* path) {
	if (fsloginf.ops2log & FSOP_ERR)
		logfsop(opcode, result, path, UNKNOWN_FD_NR, UNKNOWN_V_MODE, UNKNOWN_V_UID, UNKNOWN_V_GID, UNKNOWN_V_SIZE);
}

void logfserr_nopath(int opcode, int result) {
	logfserr(opcode, result, NULL);
}

void logfsop(int opcode, int result, char* path, int fd_nr, mode_t v_mode, uid_t v_uid, gid_t v_gid, off_t v_size) {
	if (path && fd_nr == EVENTS_FD_NR && strncmp(EVENTS_PATH, path, PATH_MAX) == 0)
		return;

	if (fsloginf.ops2log & opcode) {
		int next = (fsloginf.start + fsloginf.len) % NR_FSLOGREC;

		fslog[next].timestamp = time(NULL);
		fslog[next].opcode = opcode;
		fslog[next].result = result;

		if (path) {
			(void)strncpy(fslog[next].path, path, PATH_MAX - 1);
			fslog[next].path[PATH_MAX - 1] = '\0';
		}
		else {
			fslog[next].path[0] = '\0';
		}

		fslog[next].fd_nr = fd_nr;
		fslog[next].v_mode = v_mode;
		fslog[next].v_uid = v_uid;
		fslog[next].v_gid = v_gid;
		fslog[next].v_size = v_size;

		if (fsloginf.len == NR_FSLOGREC)
			fsloginf.start = (fsloginf.start + 1) % NR_FSLOGREC;
		else
			fsloginf.len++;
	}
}

void logfsop_nopath(int opcode, int result, int fd_nr, mode_t v_mode, uid_t v_uid, gid_t v_gid, off_t v_size) {
	logfsop(opcode, result, NULL, fd_nr, v_mode, v_uid, v_gid, v_size);
}

