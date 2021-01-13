/**
 * Contains a program that runs a game.
 *
 * @author Alexander Noddings <b7029792@ncl.ac.uk>
 */

#include <iostream>
#include "Point.h"
#include "MathUtil.h"
#include <ctime>
#include "Bishop.h"
#include "Queen.h"
#include "Rook.h"
#include "Game.h"
#include <vector>
#include <algorithm>

const double board_size = 20;

// Max movement which can be made on the x axis, y axis, and when moving diagonally.
const double max_move_x = 6;
const double max_move_y = 6;
const double max_move_d = sqrt(max_move_x * max_move_x + max_move_y * max_move_y);

// -1 will wrap back up to a ulonglong max
const unsigned long long max_moves = 250;

using namespace std;

// generates a random int i such that min <= i <= max
int randi(int min, int max)
{
	const int diff = max - min + 1;
	// rand mod n is not evenly distributed, but that doesn't matter for this application
	const int random = rand() % diff;
	return min + random;
}

// generates a random double d such that min <= i <= max
double randd(double min, double max)
{
	const double diff = max - min;
	const double random = rand() / static_cast<double>(RAND_MAX);
	return min + (random * diff);
}

// generates a random bool
bool randb()
{
	return randi(0, 1) == 1;
}

Point generate_lateral_move(const double current_x, const double current_y, const double size)
{
	const double distance = randd(-max_move_x, max_move_x);
	double new_x = current_x;
	double new_y = current_y;
	if (randb())
		new_x = clamp(0, current_x + distance, board_size - size);
	else
		new_y = clamp(0, current_y + distance, board_size - size);
	return Point(new_x, new_y);
}

Point generate_diagonal_move(const double current_x, const double current_y, const double size)
{
	const double distance = randd(-max_move_d, max_move_d);
	const double new_x = clamp(0, current_x + distance, board_size - size);
	const double new_y = clamp(0, current_y + (randb() ? distance : -distance), board_size - size);
	return Point(new_x, new_y);
}

Point generate_move(const Piece* piece)
{
	Point new_position = Point(0, 0);
	if (piece->get_piece_type() == PieceType::bishop)
		new_position = generate_diagonal_move(piece->get_x(), piece->get_y(), 1);
	else if (piece->get_piece_type() == PieceType::rook)
		new_position = generate_lateral_move(piece->get_x(), piece->get_y(), 2);
	else
		new_position = randb()
		? generate_diagonal_move(piece->get_x(), piece->get_y(), 1)
		: generate_lateral_move(piece->get_x(), piece->get_y(), 1);
	return new_position;
}

int main()
{
	// ensure rand is seeded. seeding by time(0) is not secure but that isn't an issue here.
	srand(time(0));

	// setup pieces
	vector<Piece*> pieces = vector<Piece*>();
	pieces.push_back(new Rook(0, 0));
	pieces.push_back(new Rook(18, 0));
	pieces.push_back(new Rook(0, 18));
	pieces.push_back(new Rook(18, 18));

	pieces.push_back(new Rook(2, 4));
	pieces.push_back(new Rook(16, 4));
	pieces.push_back(new Rook(2, 14));
	pieces.push_back(new Rook(16, 14));

	pieces.push_back(new Queen(5, 1));
	pieces.push_back(new Queen(5, 7));
	pieces.push_back(new Queen(5, 12));
	pieces.push_back(new Queen(5, 18));

	pieces.push_back(new Queen(15, 1));
	pieces.push_back(new Queen(15, 7));
	pieces.push_back(new Queen(15, 12));
	pieces.push_back(new Queen(15, 18));

	pieces.push_back(new Bishop(8, 1));
	pieces.push_back(new Bishop(8, 7));
	pieces.push_back(new Bishop(8, 12));
	pieces.push_back(new Bishop(8, 18));

	pieces.push_back(new Bishop(13, 1));
	pieces.push_back(new Bishop(13, 7));
	pieces.push_back(new Bishop(13, 12));
	pieces.push_back(new Bishop(13, 18));

	// add them to the board
	Game game = Game(board_size);
	for (auto it = pieces.begin(); it != pieces.end(); ++it)
		game.add_piece(*it);

	// track capture numbers
	int rook_captures = 0;
	int queen_captures = 0;
	int bishop_captures = 0;

	unsigned long long moves = 0;
	while (pieces.size() > 1 && moves < max_moves)
	{
		Piece* piece = pieces.at(randi(0, pieces.size() - 1));
		const Point new_position = generate_move(piece);
		if (game.is_valid_move(piece, new_position))
		{
			moves++;
			Piece* collided_piece = game.make_move(piece, new_position);

			if (collided_piece != nullptr)
			{
				if (piece->get_piece_type() == PieceType::bishop)
					bishop_captures++;
				else if (piece->get_piece_type() == PieceType::rook)
					rook_captures++;
				else if(piece->get_piece_type() == PieceType::queen)
					queen_captures++;
				cout << *piece << " collided with and removed " << *collided_piece << endl;
				pieces.erase(remove(pieces.begin(), pieces.end(), collided_piece), pieces.end());
				delete collided_piece;
			}
		}
	}

	if (moves == max_moves)
		cout << "Game ended as it hit max moves. " << pieces.size() << " pieces remain." << endl;
	else
		cout << "Game ended as only one piece remains. " << moves << " moves were taken." << endl;

	cout << "Rook captured: " << rook_captures << " pieces." << endl;
	cout << "Queen captured: " << queen_captures << " pieces." << endl;
	cout << "Bishop captured: " << bishop_captures << " pieces." << endl;
}