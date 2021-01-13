/**
 * Contains the definition of a game which tracks and moves pieces.
 *
 * @author Alexander Noddings <b7029792@ncl.ac.uk>
 */

#pragma once
#include "Piece.h"
#include <vector>

using namespace std;

class Game final
{
public:
	explicit Game(double board_size);
	~Game();

	vector<Piece*> get_pieces() const;
	bool is_piece_tracked(const Piece* piece) const;
	void add_piece(Piece* piece);
	bool is_valid_move(const Piece* piece, const double new_x, const double new_y) const;
	bool is_valid_move(const Piece* piece, const Point new_position) const;
	Piece* make_move(Piece* piece, const double new_x, const double new_y);
	// helper which calls make_move(Piece*, double, double) given a Piece*, Point
	Piece* make_move(Piece* piece, const Point new_position);
private:
	double board_size_;
	vector<Piece*> pieces_;
};

