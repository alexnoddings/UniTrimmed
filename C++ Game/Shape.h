/**
 * Contains the definition of a shape.
 *
 * @author Alexander Noddings <b7029792@ncl.ac.uk>
 */

#pragma once
#include "Point.h"

class Shape
{
public:
	explicit Shape(const double top_left_x, const double top_left_y, const double width);
	Shape(const Shape& other);
	Shape(Shape&& other) noexcept;
	virtual ~Shape();

	double get_top_left_x() const;
	double get_top_left_y() const;
	double get_width() const;

	Point get_top_left() const;
	Point get_top_right() const;
	Point get_bottom_left() const;
	Point get_bottom_right() const;
protected:
	double top_left_x_;
	double top_left_y_;
	double width_;
};

