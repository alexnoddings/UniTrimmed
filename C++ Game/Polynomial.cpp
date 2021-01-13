#include "Polynomial.h"
#include <iostream>
#include <algorithm>
#include <cassert>

using namespace std;

Polynomial::Polynomial(const int degree)
	: degree(degree)
{
	if (degree < 0)
		throw invalid_argument("degree must be greater than or equal to 0");
	this->coefficients = new double[degree + 1];
	// zero out all coefficients
	for (auto i = 0; i <= degree; i++)
		coefficients[i] = 0;
}

Polynomial::Polynomial(const Polynomial& other)
	: degree(other.degree)
{
	this->coefficients = new double[degree + 1];
	for (auto i = 0; i <= degree; i++)
		coefficients[i] = other.coefficients[i];
}

Polynomial::Polynomial(Polynomial&& other) noexcept
	: degree(other.degree)
{
	this->coefficients = new double[degree + 1];
	for (auto i = 0; i <= degree; i++)
		coefficients[i] = other.coefficients[i];
}

Polynomial::~Polynomial()
{
	// need to free the coefficients from the heap
	delete[] coefficients;
}

void Polynomial::set_coefficient(const int degree, const double coefficient) noexcept(false)
{
	// !(0 <= degree <= this->degree)
	if (degree < 0 || this->degree < degree)
		throw invalid_argument("degree out of bounds");
	coefficients[degree] = coefficient;
}

double Polynomial::get_coefficient(const int degree) const noexcept(false)
{
	// !(0 <= degree <= this->degree)
	if (degree < 0 || this->degree < degree)
		throw invalid_argument("degree out of bounds");
	return coefficients[degree];
}

int Polynomial::get_degree() const
{
	return degree;
}

double Polynomial::calculate(const double x) const
{
	double total = 0;
	for (auto i = 0; i <= degree; i++)
		total += coefficients[i] * pow(x, i);
	return total;
}

bool operator==(const Polynomial& lhs, const Polynomial& rhs)
{
	// 0x + 2 == 2, cannot judge purely on degrees
	// instead need to check if the missing coefficients are 0
	// if both are of same degree, just ensure values are the same
	if (lhs.degree == rhs.degree)
	{
		for (auto degree = 0; degree <= lhs.degree; degree++)
			if (lhs.coefficients[degree] != rhs.coefficients[degree])
				return false;
		return true;
	}
	// degree alone cannot be relied on. 0x^2 + 2x + 1 == 2x + 1, therefore must check extra coefficients are 0
	if (lhs.degree > rhs.degree)
	{
		for (auto degree = 0; degree <= lhs.degree; degree++)
			if (degree > rhs.degree)
				if (lhs.coefficients[degree] != 0) return false;
		return true;
	}
	// Keep it simple and just swap to hit the above block
	return operator==(rhs, lhs);
}

bool operator!=(const Polynomial& lhs, const Polynomial& rhs)
{
	// Just return not equal to make it easy
	return !(lhs == rhs);
}

Polynomial& Polynomial::operator=(const Polynomial& other)
{
	if (this == &other)
		return *this;
	degree = other.degree;
	delete[] coefficients;
	this->coefficients = new double[degree + 1];
	for (auto i = 0; i <= degree; i++)
		coefficients[i] = other.coefficients[i];
	return *this;
}

Polynomial& Polynomial::operator=(Polynomial&& other) noexcept
{
	if (this == &other)
		return *this;
	degree = other.degree;
	delete[] coefficients;
	this->coefficients = new double[degree + 1];
	for (auto i = 0; i <= degree; i++)
		coefficients[i] = other.coefficients[i];
	return *this;
}

ostream& operator<<(ostream& os, const Polynomial& polynomial)
{
	for (auto degree = polynomial.get_degree(); degree >= 0; degree--)
	{
		os << polynomial.get_coefficient(degree);
		if (degree > 0)
			os << "x^" << degree << " + ";
	}
	return os;
}

istream& operator>>(istream& is, Polynomial& polynomial)
{
	is >> polynomial.degree;
	delete[] polynomial.coefficients;
	polynomial.coefficients = new double[polynomial.degree + 1];
	for (auto i = 0; i <= polynomial.degree; i++)
		polynomial.coefficients[i] = 0;
	return is;
}

Polynomial Polynomial::operator+(const Polynomial& rhs) const
{
	const auto biggest_degree = max(this->degree, rhs.degree);
	const auto polynomial = Polynomial(biggest_degree);
	for (auto degree = 0; degree <= biggest_degree; degree++)
	{
		double coefficient = 0;
		if (degree <= this->degree)
			coefficient += this->coefficients[degree];
		if (degree <= rhs.degree)
			coefficient += rhs.coefficients[degree];
		polynomial.coefficients[degree] = coefficient;
	}
	return polynomial;
}

Polynomial Polynomial::operator-(const Polynomial& rhs) const
{
	const auto biggest_degree = max(this->degree, rhs.degree);
	const auto polynomial = Polynomial(biggest_degree);
	for (auto degree = 0; degree <= biggest_degree; degree++)
	{
		double coefficient = 0;
		if (degree <= this->degree)
			coefficient = this->coefficients[degree];
		if (degree <= rhs.degree)
			coefficient -= rhs.coefficients[degree];
		polynomial.coefficients[degree] = coefficient;
	}
	return polynomial;
}

Polynomial Polynomial::operator*(const int rhs) const
{
	const auto polynomial = Polynomial(*this);
	for (auto degree = 0; degree <= polynomial.degree; degree++)
		polynomial.coefficients[degree] *= rhs;
	return polynomial;
}

Polynomial Polynomial::operator*(const double rhs) const
{
	const auto polynomial = Polynomial(*this);
	for (auto degree = 0; degree <= polynomial.degree; degree++)
		polynomial.coefficients[degree] *= rhs;
	return polynomial;
}

Polynomial Polynomial::operator*(const Polynomial& rhs) const
{
	const auto polynomial = Polynomial(this->degree + rhs.degree);
	for (auto ld = 0; ld <= this->degree; ld++)
		for (auto rd = 0; rd <= rhs.degree; rd++)
			polynomial.coefficients[ld + rd] += this->coefficients[ld] * rhs.coefficients[rd];
	return polynomial;
}

Polynomial Polynomial::operator/(const double rhs) const
{
	const auto polynomial = Polynomial(*this);
	for (auto degree = 0; degree <= polynomial.degree; degree++)
		polynomial.coefficients[degree] /= rhs;
	return polynomial;
}

Polynomial& Polynomial::operator+=(const Polynomial& rhs)
{
	if (rhs.degree > this->degree)
	{
		const auto new_coefficients = new double[rhs.degree + 1];
		for (auto d = 0; d <= this->degree; d++)
			new_coefficients[d] = this->coefficients[d] + rhs.coefficients[d];
		for (auto d = this->degree + 1; d <= rhs.degree; d++)
			new_coefficients[d] = rhs.coefficients[d];
		this->degree = rhs.degree;
		delete[] this->coefficients;
		this->coefficients = new_coefficients;
	}
	else
		for (auto d = 0; d <= rhs.degree; d++)
			this->coefficients[d] += rhs.coefficients[d];
	return *this;
}

Polynomial& Polynomial::operator-=(const Polynomial& rhs)
{
	if (rhs.degree > this->degree)
	{
		const auto new_coefficients = new double[rhs.degree + 1];
		for (auto d = 0; d <= this->degree; d++)
			new_coefficients[d] = this->coefficients[d] - rhs.coefficients[d];
		for (auto d = this->degree + 1; d <= rhs.degree; d++)
			new_coefficients[d] = - rhs.coefficients[d];
		this->degree = rhs.degree;
		delete[] this->coefficients;
		this->coefficients = new_coefficients;
	}
	else
		for (auto d = 0; d <= rhs.degree; d++)
			this->coefficients[d] -= rhs.coefficients[d];
	return *this;
}

Polynomial& Polynomial::operator*=(const int rhs)
{
	for (auto d = 0; d <= degree; d++)
		coefficients[d] *= rhs;
	return *this;
}

Polynomial& Polynomial::operator*=(const double rhs)
{
	for (auto d = 0; d <= degree; d++)
		coefficients[d] *= rhs;
	return *this;
}

Polynomial& Polynomial::operator*=(const Polynomial& rhs)
{
	const auto new_degree = this->degree + rhs.degree;
	const auto new_coefficients = new double[new_degree + 1];
	for (auto d = 0; d <= new_degree; d++)
		new_coefficients[d] = 0;
	for (auto ld = 0; ld <= this->degree; ld++)
		for (auto rd = 0; rd <= rhs.degree; rd++)
			new_coefficients[ld + rd] += this->coefficients[ld] * rhs.coefficients[rd];
	this->degree = new_degree;
	delete[] this->coefficients;
	this->coefficients = new_coefficients;
	return *this;
}

Polynomial& Polynomial::operator/=(const double rhs)
{
	for (auto d = 0; d <= degree; d++)
		coefficients[d] /= rhs;
	return *this;
}
