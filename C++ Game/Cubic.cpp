#include "Cubic.h"
#include <iostream>

using namespace std;

Cubic::Cubic()
{
	coefficient0 = 0;
	coefficient1 = 0;
	coefficient2 = 0;
	coefficient3 = 0;
}

Cubic::Cubic(const Cubic& other)
	: coefficient0(other.coefficient0),
	coefficient1(other.coefficient1),
	coefficient2(other.coefficient2),
	coefficient3(other.coefficient3)
{
}

Cubic::Cubic(Cubic&& other) noexcept
	: coefficient0(other.coefficient0),
	coefficient1(other.coefficient1),
	coefficient2(other.coefficient2),
	coefficient3(other.coefficient3)
{
}

Cubic::~Cubic() = default;

void Cubic::set_coefficient(const int degree, const double coefficient) noexcept(false)
{
	// !(0 <= degree <= 3)
	switch (degree)
	{
	case 0:
		coefficient0 = coefficient;
		break;
	case 1:
		coefficient1 = coefficient;
		break;
	case 2:
		coefficient2 = coefficient;
		break;
	case 3:
		coefficient3 = coefficient;
		break;
	default:
		throw invalid_argument("degree out of bounds");
	}
}

double Cubic::get_coefficient(const int degree) const noexcept(false)
{
	// !(0 <= degree <= 3)
	switch (degree)
	{
	case 0:
		return coefficient0;
	case 1:
		return coefficient1;
	case 2:
		return coefficient2;
	case 3:
		return coefficient3;
	default:
		throw invalid_argument("degree out of bounds");
	}
}

double Cubic::calculate(const double x) const
{
	return coefficient3 * pow(x, 3) +
		coefficient2 * pow(x, 2) +
		coefficient1 * x +
		coefficient0;
}

bool operator==(const Cubic& lhs, const Cubic& rhs)
{
	return lhs.coefficient0 == rhs.coefficient0 &&
		lhs.coefficient1 == rhs.coefficient1 &&
		lhs.coefficient2 == rhs.coefficient2 &&
		lhs.coefficient3 == rhs.coefficient3;
}

bool operator!=(const Cubic& lhs, const Cubic& rhs)
{
	// Just return not equal to make it easy
	return !(lhs == rhs);
}

Cubic& Cubic::operator=(const Cubic& other)
{
	if (this == &other)
		return *this;
	coefficient0 = other.coefficient0;
	coefficient1 = other.coefficient1;
	coefficient2 = other.coefficient2;
	coefficient3 = other.coefficient3;
	return *this;
}

Cubic& Cubic::operator=(Cubic&& other) noexcept
{
	if (this == &other)
		return *this;
	coefficient0 = other.coefficient0;
	coefficient1 = other.coefficient1;
	coefficient2 = other.coefficient2;
	coefficient3 = other.coefficient3;
	return *this;
}

ostream& operator<<(ostream& os, const Cubic& cubic)
{
	os << cubic.coefficient3 << "x^3 + "
		<< cubic.coefficient2 << "x^2 + "
		<< cubic.coefficient1 << "x + "
		<< cubic.coefficient0;
	return os;
}

istream& operator>>(istream& is, Cubic& cubic)
{
	is >> cubic.coefficient3;
	is >> cubic.coefficient2;
	is >> cubic.coefficient1;
	is >> cubic.coefficient0;
	return is;
}

Cubic Cubic::operator+(const Cubic& rhs) const
{
	auto cubic = Cubic();
	cubic.coefficient0 = this->coefficient0 + rhs.coefficient0;
	cubic.coefficient1 = this->coefficient1 + rhs.coefficient1;
	cubic.coefficient2 = this->coefficient2 + rhs.coefficient2;
	cubic.coefficient3 = this->coefficient3 + rhs.coefficient3;
	return cubic;
}

Cubic Cubic::operator-(const Cubic& rhs) const
{
	auto cubic = Cubic();
	cubic.coefficient0 = this->coefficient0 - rhs.coefficient0;
	cubic.coefficient1 = this->coefficient1 - rhs.coefficient1;
	cubic.coefficient2 = this->coefficient2 - rhs.coefficient2;
	cubic.coefficient3 = this->coefficient3 - rhs.coefficient3;
	return cubic;
}

Cubic Cubic::operator*(const int rhs) const
{
	auto cubic = Cubic();
	cubic.coefficient0 = this->coefficient0 * rhs;
	cubic.coefficient1 = this->coefficient1 * rhs;
	cubic.coefficient2 = this->coefficient2 * rhs;
	cubic.coefficient3 = this->coefficient3 * rhs;
	return cubic;
}

Cubic Cubic::operator*(const double rhs) const
{
	auto cubic = Cubic();
	cubic.coefficient0 = this->coefficient0 * rhs;
	cubic.coefficient1 = this->coefficient1 * rhs;
	cubic.coefficient2 = this->coefficient2 * rhs;
	cubic.coefficient3 = this->coefficient3 * rhs;
	return cubic;
}

Cubic Cubic::operator/(const double rhs) const
{
	auto cubic = Cubic();
	cubic.coefficient0 = this->coefficient0 / rhs;
	cubic.coefficient1 = this->coefficient1 / rhs;
	cubic.coefficient2 = this->coefficient2 / rhs;
	cubic.coefficient3 = this->coefficient3 / rhs;
	return cubic;
}

Cubic& Cubic::operator+=(const Cubic& rhs)
{
	this->coefficient0 += rhs.coefficient0;
	this->coefficient1 += rhs.coefficient1;
	this->coefficient2 += rhs.coefficient2;
	this->coefficient3 += rhs.coefficient3;
	return *this;
}

Cubic& Cubic::operator-=(const Cubic& rhs)
{
	this->coefficient0 -= rhs.coefficient0;
	this->coefficient1 -= rhs.coefficient1;
	this->coefficient2 -= rhs.coefficient2;
	this->coefficient3 -= rhs.coefficient3;
	return *this;
}

Cubic& Cubic::operator*=(const int rhs)
{
	this->coefficient0 *= rhs;
	this->coefficient1 *= rhs;
	this->coefficient2 *= rhs;
	this->coefficient3 *= rhs;
	return *this;
}

Cubic& Cubic::operator*=(const double rhs)
{
	this->coefficient0 *= rhs;
	this->coefficient1 *= rhs;
	this->coefficient2 *= rhs;
	this->coefficient3 *= rhs;
	return *this;
}

Cubic& Cubic::operator/=(const double rhs)
{
	this->coefficient0 /= rhs;
	this->coefficient1 /= rhs;
	this->coefficient2 /= rhs;
	this->coefficient3 /= rhs;
	return *this;
}
