#include <stdbool.h>
#include <errno.h>
#include <limits.h>
#include <stdlib.h>
#include <stdio.h>
#include <fcntl.h>
#include <unistd.h>
#include <string.h>

#include "obj_store.h"
#include "integer.h"

static Integer _add(Integer self, Integer i);
static Integer _subtract(Integer self, Integer i);
static Integer _multiply(Integer self, Integer i);
static Integer _divide(Integer self, Integer i);
static Integer _modulo(Integer self, Integer i);
static int _get_value(Integer self);

static Integer _IntMax = NULL;
static Integer _IntMin = NULL;

static char* STR_REP_FMT = "int:%d\n";

static Integer _is_cached(int value) {
	if (_IntMax && _IntMax->_val == value)
		return _IntMax;

	if (_IntMin && _IntMin->_val == value)
		return _IntMin;

	return NULL;
}

static void _cache(Integer i) {
	if (!i)
		return;

	if (!_IntMax && i->_val == INT_MAX)
		_IntMax = i;

	if (!_IntMin && i->_val == INT_MIN)
		_IntMin = i;
}

static char* _new_str_rep(void* obj) {
	char* str_rep = NULL;

	if (obj) {
		Integer i = (Integer)obj;
		(void)asprintf(&str_rep, STR_REP_FMT, i->_val);
	}

	return str_rep;
}

Integer newInteger(int value) {
	Integer self = _is_cached(value);

	if (self) return self;

	self = (Integer)malloc(sizeof(struct integer));

	if (self) {
		self->_val = value;

		self->add = _add;
		self->subtract = _subtract;
		self->multiply = _multiply;
		self->divide = _divide;
		self->modulo = _modulo;

		self->get_value = _get_value;

		if (ostore_is_on() && !store_obj(self, _new_str_rep)) {

			free(self);
			self = NULL;
		}
		else {
			_cache(self);
		}
	}

	return self;
}

void deleteInteger(Integer* ai) {
	if (ai && *ai != _IntMax && *ai != _IntMin) {
		if (ostore_is_on())
			unlink_obj(*ai);

		free(*ai);
		*ai = NULL;
	}
}

Integer IntMax() {
	if (!_IntMax)
		_IntMax = newInteger(INT_MAX);

	return _IntMax;
}

Integer IntMin() {
	if (!_IntMin)
		_IntMax = newInteger(INT_MIN);

	return _IntMin;
}

Integer _add(Integer self, Integer i) {
	if (!self || !i) {
		errno = EINVAL;
		return NULL;
	}

	Integer r = NULL;

	if ((self->_val < 0 && i->_val > 0) || (self->_val > 0 && i->_val < 0)) {
		r = newInteger(self->_val + i->_val);
	}
	else if (self->_val >= 0 && (self->_val <= INT_MAX - i->_val)) {
		r = newInteger(self->_val + i->_val);
	}
	else if (self->_val <= 0 && (self->_val >= -INT_MAX - (i->_val + 1))) {
		r = newInteger(self->_val + i->_val);
	}

	if (!r) errno = ERANGE;

	return r;
}

Integer _subtract(Integer self, Integer i) {
	if (!self || !i) {
		errno = EINVAL;
		return NULL;
	}

	if (i->_val < 0 && self->_val > INT_MAX + i->_val)
	{
		errno = ERANGE;
		return NULL;
	}

	if (i->_val > 0 && self->_val < INT_MIN + i->_val)
	{
		errno = ERANGE;
		return NULL;
	}

	return newInteger(self->_val - i->_val);
}

Integer _multiply(Integer self, Integer i) {
	if (!self || !i) {
		errno = EINVAL;
		return NULL;
	}

	Integer r = NULL;

	bool valid = false;

	if (!self->_val || !i->_val) {
		valid = true;
	}
	else if (self->_val > 0) {
		valid = i->_val > 0 ? self->_val <= INT_MAX / i->_val
			: i->_val >= INT_MIN / self->_val;
	}
	else {
		valid = i->_val > 0 ? self->_val >= INT_MIN / i->_val
			: self->_val >= INT_MAX / i->_val;
	}

	if (valid)
		r = newInteger(self->_val * i->_val);
	else
		errno = ERANGE;

	return r;
}

Integer _divide(Integer self, Integer i) {
	if (!self || !i) {
		errno = EINVAL;
		return NULL;
	}

	if (self->_val == INT_MIN && i->_val == -1) {
		errno = ERANGE;
		return NULL;
	}

	if (i->_val == 0) {
		errno = ERANGE;
		return NULL;
	}

	return newInteger(self->_val / i->_val);
}

Integer _modulo(Integer self, Integer i) {
	if (!self || !i) {
		errno = EINVAL;
		return NULL;
	}

	if (self->_val == INT_MIN && i->_val == -1) {
		errno = ERANGE;
		return NULL;
	}

	if (i->_val == 0) {
		errno = ERANGE;
		return NULL;
	}

	return newInteger(self->_val % i->_val);
}

int _get_value(Integer self) {
	if (self) {
		return self->_val;
	}
	else {
		errno = EINVAL;
		return 0;
	}
}