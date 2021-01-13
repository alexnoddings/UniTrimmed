#include <stdio.h>
#include <sys/stat.h>
#include <sys/types.h>
#include <fcntl.h>
#include <errno.h>
#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <dirent.h>
#include "obj_store.h"

#define OSTR_REP_MAX 4096

static bool ostore_on = false;

const char* OFILE_FMT = "./%s/%#zx.txt";
const char* OSTORE_DIR = "ostore";

char* _get_ofile_path(size_t oid);

void disable_ostore() {
	ostore_on = false;

	DIR* osd = opendir(OSTORE_DIR);

	if (osd) {
		struct dirent* de;

		while ((de = readdir(osd))) {
			if (de->d_name[0] != '.') {
				char* path = NULL;
				(void)asprintf(&path, "./%s/%s", OSTORE_DIR, de->d_name);
				if (path) {
					unlink(path);
					free(path);
				}
			}
		}

		closedir(osd);

		rmdir(OSTORE_DIR);
	}

	errno = 0;
}

bool enable_ostore() {
	struct stat sbuf;

	errno = 0;

	int r = stat(OSTORE_DIR, &sbuf);

	if (r) {
		if (errno == ENOENT) {
			errno = 0;
			r = mkdir(OSTORE_DIR, 0755);
		}
	}
	else if (!S_ISDIR(sbuf.st_mode)) {

		r = -1;
		errno = ENOTDIR;
	}

	ostore_on = !r;

	return ostore_on;
}

bool ostore_is_on() {
	return ostore_on;
}

bool store_obj(void* obj, char* (*new_str_rep)(void*)) {
	if (!obj || !new_str_rep) {
		errno = EINVAL;
		return false;
	}

	if (!ostore_on) {
		errno = ENOENT;
		return false;
	}

	char* ofile_path = _get_ofile_path((size_t)obj);
	if (!ofile_path) {
		errno = ENOENT;
		return false;
	}

	char* buf = new_str_rep(obj);
	size_t buf_len = strlen(buf);
	buf_len = buf_len > OSTR_REP_MAX ? OSTR_REP_MAX : buf_len;

	int file = open(ofile_path, O_RDWR | O_TRUNC | O_CREAT, 0644);

	if (file < 0) {
		free(ofile_path);
		free(buf);
		errno = file;
		return false;
	}

	ssize_t bytes_written = write(file, buf, buf_len);

	int closed = close(file);

	free(buf);

	if (bytes_written == 0) {
		unlink(ofile_path);
		free(ofile_path);
		return false;
	}
	else if (bytes_written < buf_len) {
		unlink(ofile_path);
		free(ofile_path);
		return false;
	}

	free(ofile_path);

	if (closed != 0) {
		errno = closed;
		return false;
	}

	return true;
}

void unlink_obj(void* obj) {
	if (ostore_on && obj) {
		char* ofile_path = _get_ofile_path((size_t)obj);

		if (ofile_path) {
			unlink(ofile_path);
			free(ofile_path);
		}
	}

	return;
}

char* _get_ofile_path(size_t oid) {
	char* ofile_path = NULL;

	if (oid)
		(void)asprintf(&ofile_path, OFILE_FMT, OSTORE_DIR, oid);

	return ofile_path;
}
