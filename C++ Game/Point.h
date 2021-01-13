/**
 * Contains the definition of a point in 2-d space.
 *
 * @author Alexander Noddings <b7029792@ncl.ac.uk>
 */

#pragma once
class Point
{
public:
	explicit Point(const double x, const double y);

	double get_x() const;
	double get_y() const;

	friend bool operator==(const Point& lhs, const Point& rhs);
	friend bool operator!=(const Point& lhs, const Point& rhs);
private:
	double x_;
	double y_;
};

