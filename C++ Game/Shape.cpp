/**
 * Contains the functions for a shape.
 *
 * @author Alexander Noddings <b7029792@ncl.ac.uk>
 */

#include "Shape.h"

Shape::Shape(const double top_left_x, const double top_left_y, const double width) 
: top_left_x_(top_left_x), top_left_y_(top_left_y), width_(width)
{
}

Shape::Shape(const Shape& other) : Shape(other.top_left_x_, other.top_left_y_, other.width_)
{
}

Shape::Shape(Shape&& other) noexcept : Shape(other.top_left_x_, other.top_left_y_, other.width_)
{
}

Shape::~Shape() = default;

Point Shape::get_top_left() const
{
	return Point(top_left_x_, top_left_y_);
}

Point Shape::get_top_right() const
{
	return Point(top_left_x_ + width_, top_left_y_);
}

Point Shape::get_bottom_left() const
{
	return Point(top_left_x_, top_left_y_ + width_);
}

Point Shape::get_bottom_right() const
{
	return Point(top_left_x_ + width_, top_left_y_ + width_);
}

double Shape::get_top_left_x() const
{
	return top_left_x_;
}

double Shape::get_top_left_y() const
{
	return top_left_y_;
}

double Shape::get_width() const
{
	return width_;
}
