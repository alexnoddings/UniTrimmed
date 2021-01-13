/**
 * Various useful functions.
 *
 * @author Alexander Noddings <b7029792@ncl.ac.uk>
 */

#include "MathUtil.h"
#include <cmath>
#include <cfloat>
#include <algorithm>

#ifndef DBL_EPSILON
	#define DBL_EPSILON = 2.2204460492503131E-016;
#endif

bool double_same(const double double1, const double double2)
{
	return abs(double1 - double2) < DBL_EPSILON;
}

double clamp(const double left, const double value, const double right)
{
	return left <= right
		? std::max(left, std::min(value, right))
		: clamp(right, value, left);
}

double distance_between_points(const double point1_x, const double point1_y, const double point2_x, const double point2_y)
{
	const auto dx = point1_x - point2_x;
	const auto dy = point1_y - point2_y;
	return sqrt((dx * dx) + (dy * dy));
}

double distance_between_points(const Point point1, const double point2_x, const double point2_y)
{
	return distance_between_points(point1.get_x(), point1.get_y(), point2_x, point2_y);
}

double distance_between_points(const Point point1, const Point point2)
{
	return distance_between_points(point1, point2.get_x(), point2.get_y());
}

Point closest_point_in_square(const Point point, const Square square)
{
	const double x = clamp(square.get_top_left_x(), point.get_x(), square.get_top_left_x() + square.get_width());
	const double y = clamp(square.get_top_left_y(), point.get_y(), square.get_top_left_y() + square.get_width());
	return Point(x, y);
}

bool is_point_in_square(const Point point, const Square square)
{
	return square.get_top_left_x() <= point.get_x() && point.get_x() <= square.get_top_left_x() + square.get_width()
		&& square.get_top_left_y() <= point.get_y() && point.get_y() <= square.get_top_left_y() + square.get_width();
}