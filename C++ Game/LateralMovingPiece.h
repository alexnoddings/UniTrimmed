/**
 * Contains the definition of a piece which is capable of moving horizontally/vertically.
 *
 * @author Alexander Noddings <b7029792@ncl.ac.uk>
 */

#pragma once
#include "Piece.h"

class LateralMovingPiece: public virtual Piece
{
public:
	explicit LateralMovingPiece(const double x, const double y);
	LateralMovingPiece(const LateralMovingPiece& other);
	LateralMovingPiece(LateralMovingPiece&& other) noexcept;
	virtual ~LateralMovingPiece();

	bool is_valid_move(const double new_x, const double new_y) const override;
};

