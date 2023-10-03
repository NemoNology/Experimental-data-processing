import matplotlib.pyplot as plt
import numpy as np
import tabulate as tb

Data = np.array([
    19.8, 19.1, 19.3, 18.8, 20.2, 20.8, 20.7, 19.7, 19.6, 19.2,
    20.9, 20.9, 20.2, 19.6, 20.4, 20.4, 20.2, 20.4, 18.9, 19.7,
    19.8, 20.6, 20.7, 19.7, 20.3, 19.8, 20.4, 20.3, 20.6, 20.5,
    20.4, 20.5, 20.3, 20.5, 20.2, 20.5, 20.7, 21.0, 20.4, 20.8,
    20.5, 20.4, 20.6, 21.0, 20.4, 20.4, 20.3, 19.7, 19.9, 20.1
], type(float))

lenData = len(Data)
# print(tb.tabulate(np.array_split(Data, 10)))

sortedData = np.sort(Data)
orderedData, frequency = np.unique(sortedData, return_counts=True)
lenOrderedData = len(orderedData)
barColumnSize = (orderedData[0] - orderedData[-1]) / lenOrderedData

# print(tb.tabulate([uniqueData, frequency], showindex=['Xi', 'Frequency']))

bar = plt.bar(orderedData, frequency,
              color='purple', edgecolor='k',
              width=barColumnSize)
plt.title('Discrete series')
plt.xlabel('Value')
plt.ylabel('Frequency')

R = sortedData[-1] - sortedData[0]
k = np.sqrt(lenData)
h = np.round(R / k, 1)
offset = 0.5 * h
x0 = sortedData[0] - offset
lenSeries = int(np.round(k, 1))
sessions = np.array([(.0, .0)] * lenSeries)
sessionElementsCount = np.array([1] * lenSeries)

for i in range(lenSeries):
    sessionEnd = x0 + h
    sessions[i] = (x0, sessionEnd)
    sessionElementsCount[i] = \
        np.sum(((sortedData >= x0) & (sortedData < sessionEnd)))
    x0 += h

# print(
#     tb.tabulate(
#         [[f'{session[0]:.2f} - {session[1]:.2f}' for session in sessions],
#          sessionElementsCount]))

positions = [session[0] + offset for session in sessions]
plt.bar(positions, sessionElementsCount,
        color='purple', edgecolor='k',
        width=h)
plt.plot(positions, sessionElementsCount,
         color="k", linewidth=2)
plt.title('Interval series')
plt.xlabel('Interal center')
plt.ylabel('Frequency')


relatedFrequencies = np.array([np.round(fr / lenData, 2) for fr in frequency])
accumulatedRelativeFrequencies = [.0] * lenOrderedData
accumulatedRelativeFrequencies[0] = relatedFrequencies[0]
for i in range(1, lenOrderedData):
    accumulatedRelativeFrequencies[i] = \
        accumulatedRelativeFrequencies[i - 1] + relatedFrequencies[i]

# print(tb.tabulate(
#     [sortedData,
#      relatedFrequencies,
#      accumulatedRelativeFrequencies]))

plt.bar(orderedData, accumulatedRelativeFrequencies,
        width=(orderedData[-1] - orderedData[0]) / lenOrderedData, color='purple', edgecolor='k')
plt.plot(orderedData, accumulatedRelativeFrequencies,
         linewidth=2, color='k')
plt.title('Sum curve')
plt.xlabel('Interal center')
plt.ylabel('Frequency')

MoX, = orderedData[np.where(frequency == frequency.max())]
MeX = .0
half = lenOrderedData / 2
if (lenOrderedData % 2 == 0):
    MeX = np.round((orderedData[half] + orderedData[half + 1]) / 2, 2)
else:
    MeX = orderedData[(int(np.round(half, 0)))]

u = np.array((orderedData - MoX) / h)

res = np.array([
    [orderedData[i], frequency[i], u[i],
     frequency[i] * u[i],
     frequency[i] * u[i]**2,
     frequency[i] * u[i]**3,
     frequency[i] * u[i]**4]
    for i in range(lenOrderedData)])

m1, m2, m3, m4 = 0, 0, 0, 0

for i in range(lenOrderedData):
    m1 += res[i, 3]
    m2 += res[i, 4]
    m3 += res[i, 5]
    m4 += res[i, 6]

m1 /= lenData
m2 /= lenData
m3 /= lenData
m4 /= lenData

headers = [
    'Xi', 'Ni', 'Ui', 'Ni * Ui',
     'Ni * Ui^2', 'Ni * Ui^3', 'Ni * Ui^4']
# print(tb.tabulate(res, headers=headers, floatfmt='.2f'))

# print(f'M1: {m1:.2f}')
# print(f'M2: {m2:.2f}')
# print(f'M3: {m3:.2f}')
# print(f'M4: {m4:.2f}')
s_2 = (m2 - m1**2) * h**2
s = np.sqrt(s_2)
cA = m1 * h + MoX
# print(f'Chosen avarage (X̄): {cA:.2f}')
# print(f'S^2 = D(X): {s_2:.2f}')
# print(f'S: {s:.2f}')
# print(f'V: {s / cA:.2f}')
m_3 = (m3 - 3 * m2 * m1 + 2 * m1**3) * h**3
m_4 = (m4 - 4 * m3 * m1 + 6 * m2 * m1**2 - 3 * m1**4) * h**4
# print(f'm3: {m3:.2f}')
# print(f'm4: {m_4:.2f}')
# print(f'As: {m_3 / s**3:.2f}')
# print(f'Ex: {m_4 / s**4 - 3:.2f}')

ty = 1.984
q = 0.143
# print(f'{cA - s / lenOrderedData * ty:.2f} < a < {cA + s / lenOrderedData * ty:.2f}')
# print(f'{s * (1 - q):.2f} < σ < {s * (1 + q):.2f}')