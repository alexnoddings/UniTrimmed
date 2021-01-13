/**
 * Functions for calculating intersection.
 *
 * @author Alexander Noddings <b7029792@ncl.ac.uk>
 */

#include "Intersection.h"
#include "MathUtil.h"

const double max_step_size = 0.1;

bool do_intersect(const Circle circle, const Square square)
{
	const Point closest_point = closest_point_in_square(circle.get_centre(), square);
	return distance_between_points(closest_point, circle.get_centre()) <= circle.get_radius();
}

bool do_intersect(const Square square, const Circle circle)
{
	return do_intersect(circle, square);
}

bool do_intersect(const Square square1, const Square square2)
{
	return is_point_in_square(square1.get_top_left(), square2)
		|| is_point_in_square(square1.get_top_right(), square2)
		|| is_point_in_square(square1.get_bottom_left(), square2)
		|| is_point_in_square(square1.get_bottom_right(), square2);
}

bool do_intersect(const Circle circle1, const Circle circle2)
{
	return distance_between_points(circle1.get_centre(), circle2.get_centre()) <= circle1.get_radius() + circle2.get_radius();
}

bool do_intersect(const Circle circle, const Piece* piece2)
{
	return piece2->get_piece_type() == PieceType::rook
		? do_intersect(circle, Square(piece2->get_x(), piece2->get_y(), 1))
		: do_intersect(circle, Circle(piece2->get_x(), piece2->get_y(), 2));
}

bool do_intersect(const Square square, const Piece* piece2)
{
	return piece2->get_piece_type() == PieceType::rook
		? do_intersect(square, Square(piece2->get_x(), piece2->get_y(), 1))
		: do_intersect(square, Circle(piece2->get_x(), piece2->get_y(), 2));
}

bool do_intersect(const Piece* piece1, const Piece* piece2)
{
	return piece1->get_piece_type() == PieceType::rook
		? do_intersect(Square(piece1->get_x(), piece2->get_y(), 1), piece2)
		: do_intersect(Circle(piece1->get_x(), piece2->get_y(), 2), piece2);
}

bool will_intersect(const Circle circle, const Point new_position, const Square square)
{
	return do_intersect(Circle(new_position, circle.get_width()), square);
}

bool will_intersect(const Square square, const Point new_position, const Circle circle)
{
	return do_intersect(Square(new_position, square.get_width()), circle);
}

bool will_intersect(const Square square1, const Point new_position, const Square square2)
{
	return do_intersect(Square(new_position, square1.get_width()), square2);
}

bool will_intersect(const Circle circle1, const Point new_position, const Circle circle2)
{
	return do_intersect(Circle(new_position, circle1.get_width()), circle2);
}

bool will_intersect(const Circle moving_circle, const Point new_position, const Piece* static_piece)
{
	return static_piece->get_piece_type() == PieceType::rook
		? will_intersect(moving_circle, new_position, Square(static_piece->get_x(), static_piece->get_y(), 1))
		: will_intersect(moving_circle, new_position, Circle(static_piece->get_x(), static_piece->get_y(), 2));
}

bool will_intersect(const Square moving_square, const Point new_position, const Piece* static_piece)
{
	 return static_piece->get_piece_type() == PieceType::rook
		? will_intersect(moving_square, new_position, Square(static_piece->get_x(), static_piece->get_y(), 1))
		: will_intersect(moving_square, new_position, Circle(static_piece->get_x(), static_piece->get_y(), 2));
}

bool will_intersect(const Piece* moving_piece, const Point new_position, const Piece* static_piece)
{
	return moving_piece->get_piece_type() == PieceType::rook
		? will_intersect(Square(moving_piece->get_x(), moving_piece->get_y(), 1), new_position, static_piece)
		: will_intersect(Circle(moving_piece->get_x(), moving_piece->get_y(), 2), new_position, static_piece);
}