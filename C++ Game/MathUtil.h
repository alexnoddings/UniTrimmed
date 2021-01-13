/**
 * Various useful functions.
 *
 * @author Alexander Noddings <b7029792@ncl.ac.uk>
 */

#pragma once
#include "Point.h"
#include "Square.h"

// If two doubles are close enough to be considered the same
bool double_same(const double double1, const double double2);

// Clamps value between the left and right values. Left may be >= right, in which case the values are swapped
double clamp(const double left, const double value, const double right);

// Calculates the distance between two points
double distance_between_points(const double point1_x, const double point1_y, const double point2_x, const double point2_y);

// Calculates the distance between two points
double distance_between_points(const Point point1, const double point2_x, const double point2_y);

// Calculates the distance between two points
double distance_between_points(const Point point1, const Point point2);

// Closest point in a square to a point
Point closest_point_in_square(const Point point, const Square square);

// Calculates if a point lies within a square
bool is_point_in_square(const Point point, const Square square);