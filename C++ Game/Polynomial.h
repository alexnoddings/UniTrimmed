#include <ostream>

using namespace std;

class Polynomial
{
public:
	explicit Polynomial(const int degree);
	Polynomial(const Polynomial& other);
	Polynomial(Polynomial&& other) noexcept;
	virtual ~Polynomial();

	double get_coefficient(const int degree) const noexcept(false);
	void set_coefficient(const int degree, const double coefficient) noexcept(false);
	int get_degree() const;

	double calculate(const double x) const;
			
	friend bool operator==(const Polynomial& lhs, const Polynomial& rhs);
	friend bool operator!=(const Polynomial& lhs, const Polynomial& rhs);

	friend std::ostream& operator<<(std::ostream& os, const Polynomial& polynomial);
	friend std::istream& operator>>(std::istream& is, Polynomial& polynomial);

	Polynomial& operator=(const Polynomial& other);
	Polynomial& operator=(Polynomial&& other) noexcept;

	Polynomial operator+(const Polynomial& rhs) const;
	Polynomial& operator+=(const Polynomial& rhs);

	Polynomial operator-(const Polynomial& rhs) const;
	Polynomial& operator-=(const Polynomial& rhs);

	Polynomial operator*(const int rhs) const;
	Polynomial operator*(const double rhs) const;
	Polynomial operator*(const Polynomial& rhs) const;
	Polynomial& operator*=(int rhs);
	Polynomial& operator*=(double rhs);
	Polynomial& operator*=(const Polynomial& rhs);

	Polynomial operator/(const double rhs) const;
	Polynomial& operator/=(double rhs);

private:
	int degree;
	double* coefficients;
};
