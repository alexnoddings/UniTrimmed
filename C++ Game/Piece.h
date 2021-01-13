/**
 * Contains the definition of an abstract piece in the game.
 *
 * @author Alexander Noddings <b7029792@ncl.ac.uk>
 */

#pragma once
#include "Point.h"
#include "PieceType.h"
#include <ostream>

class Piece
{
public:
	explicit Piece(const double x, const double y);
	Piece(const Piece& other);
	Piece(Piece&& other) noexcept;
	virtual ~Piece();

	Point get_top_left_position() const;
	double get_x() const;
	double get_y() const;

	virtual bool is_valid_move(const double new_x, const double new_y) const = 0;
	// helper which calls is_valid_move(double, double) given a Point
	bool is_valid_move(const Point new_position) const;
	virtual PieceType get_piece_type() const = 0;

	friend std::ostream& operator<<(std::ostream& os, const Piece& piece);

	friend class Game;
protected:
	double x_;
	double y_;
};

