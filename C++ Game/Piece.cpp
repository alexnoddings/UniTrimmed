/**
 * Contains the functions for an abstract piece in the game.
 *
 * @author Alexander Noddings <b7029792@ncl.ac.uk>
 */

#include "Piece.h"

Piece::Piece(const double x, const double y)
{
	if (x < 0) throw std::invalid_argument("x cannot be < 0");
	if (y < 0) throw std::invalid_argument("y cannot be < 0");
	x_ = x;
	y_ = y;
}

Piece::Piece(const Piece& other) : Piece(other.x_, other.y_)
{
}

Piece::Piece(Piece && other) noexcept : Piece(other.x_, other.y_)
{
}

Piece::~Piece() = default;

Point Piece::get_top_left_position() const
{
	return Point(x_, y_);
}

double Piece::get_x() const
{
	return x_;
}

double Piece::get_y() const
{
	return y_;
}

bool Piece::is_valid_move(const Point new_position) const
{
	return is_valid_move(new_position.get_x(), new_position.get_y());
}

std::ostream& operator<<(std::ostream& os, const Piece& piece)
{
	switch (piece.get_piece_type())
	{
	case PieceType::bishop:
		os << "Bishop";
		break;
	case PieceType::queen:
		os << "Queen";
		break;
	case PieceType::rook:
		os << "Rook";
		break;
	default:
		throw std::invalid_argument("unsupported piece type");
	}
	return os << " at (" << piece.x_ << ", " << piece.y_ << ")";
}
