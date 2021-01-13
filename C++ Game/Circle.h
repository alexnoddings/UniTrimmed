/**
 * Contains the definition of a circle shape.
 *
 * @author Alexander Noddings <b7029792@ncl.ac.uk>
 */

#pragma once
#include "Point.h"
#include "Shape.h"

class Circle : public virtual Shape
{
public:
	explicit Circle(const double top_left_x, const double top_left_y, const double width);
	explicit Circle(const Point top_left, const double width);
	Circle(const Circle& other);
	Circle(Circle&& other) noexcept;
	virtual ~Circle();

	Point get_centre() const;
	double get_centre_x() const;
	double get_centre_y() const;
	double get_radius() const;
};
