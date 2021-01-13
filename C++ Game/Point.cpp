/**
 * Contains the functions for a point in 2-d space.
 *
 * @author Alexander Noddings <b7029792@ncl.ac.uk>
 */

#include "Point.h"

Point::Point(const double x, const double y) : x_(x), y_(y)
{
}

double Point::get_x() const
{
	return x_;
}

double Point::get_y() const
{
	return y_;
}

bool operator==(const Point& lhs, const Point& rhs)
{
	return lhs.x_ == rhs.x_
		&& lhs.y_ == rhs.y_;
}

bool operator!=(const Point& lhs, const Point& rhs)
{
	return !(lhs == rhs);
}