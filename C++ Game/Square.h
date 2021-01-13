/**
 * Contains the definition of a square shape.
 *
 * @author Alexander Noddings <b7029792@ncl.ac.uk>
 */

#pragma once
#include "Shape.h"

class Square : public virtual Shape
{
public:
	explicit Square(const double top_left_x, const double top_left_y, const double width);
	explicit Square(const Point top_left, const double width);
	Square(const Square& other);
	Square(Square&& other) noexcept;
	virtual ~Square();
};
