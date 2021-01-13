#include <ostream>

using namespace std;

class Cubic
{
public:
	explicit Cubic();
	Cubic(const Cubic& other);
	Cubic(Cubic&& other) noexcept;
	virtual ~Cubic();

	double get_coefficient(const int degree) const noexcept(false);
	void set_coefficient(const int degree, const double coefficient) noexcept(false);

	double calculate(const double x) const;

	friend bool operator==(const Cubic& lhs, const Cubic& rhs);
	friend bool operator!=(const Cubic& lhs, const Cubic& rhs);

	friend std::ostream& operator<<(std::ostream& os, const Cubic& cubic);
	friend std::istream& operator>>(std::istream& is, Cubic& cubic);

	Cubic& operator=(const Cubic& other);
	Cubic& operator=(Cubic&& other) noexcept;

	Cubic operator+(const Cubic& rhs) const;
	Cubic& operator+=(const Cubic& rhs);

	Cubic operator-(const Cubic& rhs) const;
	Cubic& operator-=(const Cubic& rhs);

	Cubic operator*(const int rhs) const;
	Cubic operator*(const double rhs) const;
	Cubic& operator*=(int rhs);
	Cubic& operator*=(double rhs);

	Cubic operator/(const double rhs) const;
	Cubic& operator/=(double rhs);

private:
	double coefficient0;
	double coefficient1;
	double coefficient2;
	double coefficient3;
};
