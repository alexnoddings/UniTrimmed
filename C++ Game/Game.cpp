/**
 * Contains the functions for a game which tracks and moves pieces.
 *
 * @author Alexander Noddings <b7029792@ncl.ac.uk>
 */

#include "Game.h"
#include "Intersection.h"
#include "MathUtil.h"
#include <algorithm>
#include <iostream>

// can be altered. lower = slower, more accurate, higher = faster, less accurate (may miss collisions)
const double step_size = 0.01;

Game::Game(const double board_size)
{
	if (board_size <= 2)
		throw std::invalid_argument("board_size must be greater than 2");
	board_size_ = board_size;
}

Game::~Game() = default;

vector<Piece*> Game::get_pieces() const
{
	return this->pieces_;
}

bool Game::is_piece_tracked(const Piece* piece) const
{
	return std::find(pieces_.begin(), pieces_.end(), piece) != pieces_.end();
}

void Game::add_piece(Piece* piece)
{
	if (is_piece_tracked(piece))
		throw std::invalid_argument("piece is already in the game");
	pieces_.push_back(piece);
}

bool Game::is_valid_move(const Piece* piece, const double new_x, const double new_y) const
{
	if (!is_piece_tracked(piece))
		throw std::invalid_argument("piece is not in the game");

	const int piece_size = piece->get_piece_type() == PieceType::rook ? 2 : 1;
	return 0 <= new_x && new_x + piece_size <= board_size_
		&& 0 <= new_y && new_y + piece_size <= board_size_
		&& piece->is_valid_move(new_x, new_y);
}

bool Game::is_valid_move(const Piece* piece, const Point new_position) const
{
	return is_valid_move(piece, new_position.get_x(), new_position.get_y());
}

Piece* Game::make_move(Piece* piece, const double new_x, const double new_y)
{
	if (!is_valid_move(piece, new_x, new_y))
		throw std::invalid_argument("invalid move");

	const double step_x = piece->get_x() < new_x ? step_size : -step_size;
	const double step_y = piece->get_y() < new_y ? step_size : -step_size;

	Piece* intersected_with = nullptr;

	while ((piece->get_x() != new_x || piece->get_y() != new_y) && intersected_with == nullptr)
	{
		const double stepped_x = clamp(piece->get_x(), piece->get_x() + step_x, new_x);
		const double stepped_y = clamp(piece->get_y(), piece->get_y() + step_y, new_y);
		const Point stepped_point = Point(stepped_x, stepped_y);
		piece->x_ = stepped_x;
		piece->y_ = stepped_y;

		for (auto it = pieces_.begin(); it != pieces_.end(); ++it)
			if (piece != *it)
				if (will_intersect(piece, stepped_point, *it))
					intersected_with = *it;
	}

	if (intersected_with != nullptr)
		pieces_.erase(remove(pieces_.begin(), pieces_.end(), intersected_with), pieces_.end());
	return intersected_with;
}

Piece* Game::make_move(Piece* piece, const Point new_position)
{
	return make_move(piece, new_position.get_x(), new_position.get_y());
}

