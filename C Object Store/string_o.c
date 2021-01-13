#include <stdbool.h>
#include <errno.h>
#include <limits.h>
#include <stdlib.h>
#include <stdio.h>
#include <fcntl.h>
#include <unistd.h>
#include <string.h>

#include "obj_store.h"
#include "string_o.h"

static String _add(String self, String s);
static char _char_at(String self, int posn);
static bool _equals(String self, String s);
static char* _get_value(String self, char* buf);
static int _index_of(String self, char c, int start);
static int _length(String self);

static char* STR_REP_FMT = "str:%d:%s\n";

static char* _new_str_rep(void* obj) {
	char* str_rep = NULL;

	if (obj) {
		String s = (String)obj;
		(void)asprintf(&str_rep, STR_REP_FMT, s->_len, s->_val);
	}

	return str_rep;
}

String newString(char* value) {
	if (value == NULL) {
		errno = EINVAL;
		return NULL;
	}

	String self = (String)malloc(sizeof(struct string));
	if (self) {
		size_t length = strlen(value);
		length = length > STR_LEN_MAX ? STR_LEN_MAX : length;
		self->_len = (int)length;

		if (length == 0) {
			self->_val = (char*)malloc(1 * sizeof(char));
			self->_val[0] = '\0';
		}
		else {
			self->_val = (char*)malloc(length * sizeof(char));
			strncpy(self->_val, value, length);
			self->_val[length] = '\0';
		}

		self->add = _add;
		self->char_at = _char_at;
		self->equals = _equals;
		self->get_value = _get_value;
		self->index_of = _index_of;
		self->length = _length;

		if (ostore_is_on() && !store_obj(self, _new_str_rep)) {
			free(self);
			self = NULL;
			errno = ENOMEM;
		}
	}

	return self;
}

void deleteString(String* as) {
	if (as == NULL || *as == NULL) {
		return;
	}

	if (ostore_is_on())
		unlink_obj(*as);

	char* val = (*as)->_val;
	free(val);
	free(*as);

	*as = NULL;
}

String _add(String self, String s) {
	if (!self || !s)
	{
		errno = EINVAL;
		return NULL;
	}

	int selflen = self->length(self);
	int slen = s->length(s);

	if (selflen > STR_LEN_MAX - 1) {
		selflen = STR_LEN_MAX;
		slen = 0;
	}
	else if (selflen + slen > STR_LEN_MAX - 1) {
		slen = STR_LEN_MAX - selflen;
	}

	size_t bufsize = (selflen + slen + 1) * sizeof(char);
	char* buf = (char*)malloc(bufsize);

	strncpy(buf, self->_val, (size_t)selflen);
	buf[selflen] = '\0';
	if (slen > 0)
		strncat(buf, s->_val, (size_t)slen);

	buf[selflen + slen] = '\0';

	String string = newString(buf);

	free(buf);

	return string;
}

char _char_at(String self, int posn) {
	if (!self) {
		errno = EINVAL;
		return 0;
	}

	if (posn < 0) {
		errno = EINVAL;
		return 0;
	}

	if (self->length(self) == 0) {
		if (posn != 0) {
			errno = EINVAL;
		}
		return 0;
	}

	if (posn >= self->length(self)) {
		errno = EINVAL;
		return 0;
	}

	return self->_val[posn];
}

bool _equals(String self, String s) {
	if (!self || !s) return false;

	if (self == s) return true;

	if (self->length(self) != s->length(s)) return false;

	return strncmp(self->_val, self->_val, (size_t)self->length(self)) == 0;
}

char* _get_value(String self, char* buf) {
	if (!self || !buf) {
		errno = EINVAL;
		return NULL;
	}

	int length = self->length(self);
	strncpy(buf, self->_val, (size_t)length);
	buf[length] = '\0';

	return buf;
}

int _index_of(String self, char c, int start) {
	if (!self) {
		errno = EINVAL;
		return -1;
	}

	if (start < 0) {
		errno = EINVAL;
		return -1;
	}

	if (self->length(self) == 0) {
		return -1;
	}

	if (start >= self->length(self)) {
		errno = EINVAL;
		return -1;
	}

	for (int i = start; i < self->length(self); i++) {
		if (self->_val[i] == c) {
			return i;
		}
	}

	return -1;
}

int _length(String self) {
	if (!self) {
		errno = EINVAL;
		return -1;
	}

	return self->_len;
}
