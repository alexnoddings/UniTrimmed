/**
 * Contains the functions for a circle shape.
 *
 * @author Alexander Noddings <b7029792@ncl.ac.uk>
 */

#include "Circle.h"

Circle::Circle(const double top_left_x, const double top_left_y, const double width)
	: Shape(top_left_x, top_left_y, width)
{
}

Circle::Circle(const Point top_left, const double width)
	: Circle(top_left.get_x(), top_left.get_y(), width)
{
}

Circle::Circle(const Circle& other) : Circle(other.top_left_x_, other.top_left_y_, other.width_)
{
}

Circle::Circle(Circle&& other) noexcept : Circle(other.top_left_x_, other.top_left_y_, other.width_)
{
}

Circle::~Circle() = default;

Point Circle::get_centre() const
{
	return Point(get_centre_x(), get_centre_y());
}

double Circle::get_centre_x() const
{
	return top_left_x_ + get_radius();
}

double Circle::get_centre_y() const
{
	return top_left_y_ + get_radius();
}

double Circle::get_radius() const
{
	return width_ / 2.0;
}
