import random
import itertools
import math
import matplotlib.pyplot as plt
import numpy as np
from scipy.interpolate import interp1d
import pylab

def print_flist(s):
    print("[ ", end="")
    for i in range(len(s)):
        print(f"{s[i]:.20f}; ", end="")
        if (i + 1) % 11 == 0:
            print()
    print("]")

# клацать
def white_noize():
    s = 0
    for i in range(20):
        s += random.random()
    return s - 6


def ud_4_normal_dist(M, D):
    """ Calculate coefficients
    for normal distribution
    M - math exception
    D - dispersion"""
    return M, D**0.5

def normal_dist_val(u, d):
    return white_noize() * d + u

class RandomProcess:
    def __init__(self, M: float, D: float, size=1, flag=False):
        self.func_dist_val = normal_dist_val # функция плотности
        self.ud_4_func_dist = ud_4_normal_dist # функция получения u,d коэффициентов распределения
        self.__data = []
        self.__pcoeffs = []
        self.math_except = M # мат. ожидание
        self.dispersion = D #дисперсия
        self.new_data(size)
        self.Flag = flag

    def new_data(self, size: int)-> None: # получить новые данные случайного процесса
        if size <= 0 or type(size) != int:
            raise ValueError("Bad size for process!!!")
        self.__data = [self.func_dist_val( #компоненты случайного процесса
    *self.ud_4_func_dist(self.math_except, self.dispersion)) for i in range(size)]
        # зацепка
        if len(self.__data) != len(self.__pcoeffs):
                if size > 1:
                    self.__pcoeffs = RandomProcess.new_pcoeffs(size - 1) # p-коэффициенты случайного процесса
                else:
                    self.__pcoeffs = [1]

    def size(self)-> int: # количество компонент в процессе
        return len(self.__data)

    def data(self)-> list:
        return self.__data

    def value(self): # получить значение случайного процесса
        z = 0
        for i in range(len(self.__data)):
            z += self.__data[i] * self.__pcoeffs[i]
        return z

    def __str__(self):
        s = "z = "
        a = []
        b = ""
        j = 0
        for i in range(len(self.__data) - 1, -1, -1):
            s += f"{round(abs(self.__data[i]), 2)}"
            if b == "+":
                a.append((a[j] + abs(self.__data[i])))
            else:
                if b == "-":
                    a.append((a[j] - abs(self.__data[i])))
                else:
                    a.append(abs(self.__data[i]))
            a.append(abs(self.__data[i]))
            if i != len(self.__data) - 1:
                s += f" * {round(abs(self.__pcoeffs[i]), 2)}"
                a.append((a[j] * abs(self.__pcoeffs[i])))
            if i != 0:
                if self.__data[i - 1] * self.__pcoeffs[i - 1] > 0:
                    s += " + "
                    b = "+"
                else:
                    s += " - "
                    b = "-"

            j += 1
        time = np.linspace(0, 300, len(a))
        time = list(time)
        f2 = interp1d(time, a, kind='cubic')
        pylab.subplot(1, 1, 1)
        pylab.plt.scatter(time, a)
        pylab.plt.plot(time, f2(time))
        return s

    @staticmethod
    def new_pcoeffs(size):
        """ Функция аналогия coeffs_equation_by_vieta"""
        if (size)< 1:
            raise ValueError('Bad parameters!!!')
        eq_roots = [random.randint(2, 100) * (1 if random.randint(0, 1) else -1) for i in range(size)]
        a = [0] * (len(eq_roots) + 1)
        a[len(a) - 1] = 1
        a[0] = -1 if size % 2 else 1
        for root in eq_roots:
            a[0] *= root
        a[0] = a[len(a) - 1] / a[0]
        sign = -1
        for i in range(len(eq_roots) - 1):
            inds = [e for e in range(i + 1)]
            while inds[0] <= len(eq_roots) - len(inds):
                p = 1
                for ind in inds:
                    p *= eq_roots[ind]
                a[i + 1] += p
                inds[len(inds) - 1] += 1
                for k in range(len(inds) - 1, 0, -1):
                    v = len(eq_roots) - len(inds) + k
                    if inds[k] > v:
                        inds[k - 1] += 1
                        for j in range(k - 1, len(inds) - 1):
                            inds[j + 1] = inds[j] + 1
                            a[i + 1] *= a[0] * sign
                            sign = -sign

        return a

    def correlation_func(self, t):# корркеляционная функция
        R = 0
        for i in range(len(self.__pcoeffs) - t):
            R += self.__pcoeffs[i] * self.__pcoeffs[i + t]
        return R

    def norm_correlation_func(self, t):# нормированная корредяционная функция
        return self.correlation_func(t) / self.correlation_func(0)


class RandomVectorProcess(RandomProcess):
    def __init__(self, M, D, size_vector=1, size_proc=1):
        self.__data = []
        self.__new_data(M, D, size_vector, size_proc)
        #self.Flag = flag

    def __new_data(self, M: float, D: float, size_vector: int, size_proc: int):
        if size_vector <= 0 or size_proc <= 0:
            raise ValueError("Bad size for vector process or vector process's component!!!")
        if len(self.__data) == 0:
            self.__data = [RandomProcess(M, D, size_proc, flag) for i in range(size_vector)]
        else:
            for i in range(len(self.__data)):
                n = self.__data[i].size()
                self.__data[i].new_data(n)

    def component(self, index: int):
        if index < 0 or index >= len(self.__data):
            raise IndexError("Index out of range for vector process!!!")
        c = self.__data[index]
        self.__new_data(c.math_except, c.dispersion, len(self.__data), len(c.data()))
        self.Flag = True
        return c

    def opt_way_for_get_data(self):
        """ Функция, которая ищет наиболее
        оптимальный способ
        опроса векторного случайного процесса """
        iw = 0
        m = math.inf
        miw = iw
        permutations = list(itertools.permutations([i for i in
        range(len(self.__data))]))
        for way in permutations:
            s = 0
            for k in range(len(self.__data)):
                s += math.log(abs(1 -
                                  (
                                      self.component(way[k]).norm_correlation_func(len(self.__data)) -
                                      self.component(way[k]).norm_correlation_func(len(self.__data)) ** 2
                                      )))
                s *= -0.5 
            if s < m:
                m = s
                miw = iw
            iw += 1
        return permutations[miw]

    def __str__(self):
        s = ""
        for i in range(len(self.__data)):
            s += f"{i + 1}: {self.__data[i]}"
            if i != len(self.__data) - 1:
                s += "\n"
        return s

    def size(self):
        return len(self.__data)

def get_min(way: list) -> float:
    res = 0
    n = len(way_buffer)
    for i in range(n):
        try:
            res += -0.5 * math.log(abs(1 - math.pi ** 2), abs((i + 1) * way[i]))
        except:
            return 0
    return res

flag = False
k = 6
vec_proc = RandomVectorProcess(10, 20, k, k)
print("Начальное состояние векторного случайного процесса: ")
print(vec_proc)
print()
way = vec_proc.opt_way_for_get_data()
print(f"Оптимальный способ опроса:")
for x in way:
    print(f'{x+1}, ', end="")
print("\nРезультат опроса:")
data = [None for i in range(vec_proc.size())]
for ind in way:
    data[ind] = vec_proc.component(ind)
for i in range(len(data)):
    print(f"{i+1}: {data[i]}")
print("Наиболее оптимальная подстановка:")

mins = []
way_buffer = [w + 1 for w in way]

for i in range(math.factorial(k)):
    random.shuffle(way_buffer)
    mins.append((get_min(way_buffer), list(way_buffer)))

min = mins[0]

for m in mins:
    if min[0] > m[0]:
        min = m

for mk in min[1]:
    print(f'{mk}, ', end='')
print(f"\nТак как при такой подстановке функция -0.5 * ln(1 - pi^2, i * k) достигает минимума = {min[0]:3f}")
# plt.show()