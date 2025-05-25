import matplotlib.pyplot as plt
import pandas as pd
import os

csv_file_path = os.path.join('result', 'plot_data.csv')
output_image_path = os.path.join('result', 'plot.png')

data = pd.read_csv(csv_file_path, delimiter=';')

plot_data = data[data['NTerms'] != -1].copy()

plt.figure(figsize=(10, 6))
plt.plot(plot_data['Radius'], plot_data['NTerms'], marker='o', linestyle='-')

plt.title('Зависимость числа членов ряда Тейлора от радиуса целевой зоны')
plt.xlabel('Радиус целевой зоны')
plt.ylabel('Число членов ряда')

plt.grid(True)
plt.savefig(output_image_path)
