import matplotlib.pyplot as plt
import numpy as np
import tabulate as tb

Data = np.array([
    61.2, 61.4, 60.2, 61.2, 61.3, 60.4, 61.4, 60.8, 61.2, 60.6,
    61.6, 60.2, 61.3, 60.3, 60.7, 60.9, 61.2, 60.5, 61.0, 61.4,
    61.1, 60.9, 61.5, 61.4, 60.6, 61.2, 60.1, 61.3, 61.1, 61.3,
    60.3, 61.3, 60.6, 61.7, 60.6, 61.2, 60.8, 61.3, 61.0, 61.2,
    60.5, 61.4, 60.7, 61.3, 60.9, 61.2, 61.1, 61.3, 60.9, 61.4,
    60.7, 61.2, 60.3, 61.1, 61.0, 61.5, 61.3, 61.9, 61.4, 61.3,
    61.6, 61.0, 61.7, 61.1, 60.9, 61.5, 61.6, 61.4, 61.5, 61.2,
    61.6, 61.3, 61.8, 61.1, 61.7, 60.9, 62.2, 61.1, 62.1, 61.0,
    61.5, 61.7, 62.3, 62.3, 61.7, 62.9, 62.5, 62.8, 62.6, 61.5,
    62.1, 62.6, 61.6, 62.5, 62.4, 62.3, 62.1, 62.3, 62.2, 62.1
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