   M S B A M L   `   `   `  ���� U  QLiveCharts.Wpf, Version=0.9.7.0, Culture=neutral, PublicKeyToken=0bc1f845d1ebb8dfG4clr-namespace:LiveCharts.Wpf;assembly=LiveCharts.WpfLiveCharts.Wpf   
ServiceWPF   ServiceWPF.StatisticsPage   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Dlvc4clr-namespace:LiveCharts.Wpf;assembly=LiveCharts.Wpf       N� Title$  Статистика�� N� 
Background
 �����.���� .���� $��0,0,20,0q�.+�� $��0,0,0,20q� � ColumnDefinitions �� $��* ��� $
��Auto �.+��� $%��Общая статистика��$��24(�$��Medium"��� $��1�� �� Orientation$ 
Horizontal=�.+��� -   $��Excel��$
��15,8q�$��0,0,10,0q����#2196F3
�������$��0q��� -   $
��HTML��$
��15,8q����#4CAF50
�������$��0q�� $��0,0,0,30q�$
��Left=�.+��� ���#E3F2FD �� CornerRadius$ 8�� �� Padding$ 20q�$	��180��$	��130��$��0,0,15,0q�._��  � RowDefinitions �� $
��Auto ��� $��* �.+��� $��Всего заявок��$��16(�$	��Top=��� -    ��Name$ TotalRequestsCount��$��0��$��32(�$
��Bold"�$��1��$��Center=��� ���#E8F5E9$ 8��$ 20q�$	��180��$	��130��$��0,0,15,0q�._��  �� $
��Auto ��� $��* �.+��� $��Среднее время��$��16(�$
��Wrap=�$	��Top=��� -   $ AverageCompletionTime��$��0 ч.��$��32(�$
��Bold"�$��1��$��Center=��� ���#FFF3E0$ 8��$ 20q�$	��180��$	��130��$��0,0,15,0q�._��  �� $
��Auto ��� $��* �.+��� $"��Заявок в работе��$��16(�$	��Top=��� -   $ ActiveRequestsCount��$��0��$��32(�$
��Bold"�$��1��$��Center=��� ���#E1F5FE$ 8��$ 20q�$	��180��$	��130��$��0q�._��  �� $
��Auto ��� $��* �.+��� $%��Завершено за нед.��$��16(�$
��Wrap=�$	��Top=��� -   $ CompletedThisWeek��$��0��$��32(�$
��Bold"�$��1��$��Center=��� $0��*Статистика по статусам��$��20(�$��Medium"�$��0,0,0,20q�� $	��300��$��0,0,0,40q� �� �� .+�   LiveCharts.Wpf.PieChart  -     Name$ StatusPieChart��&    LiveCharts.Wpf.Charts.Base.Chart	   LegendLocation$	 Right=�
   DataTooltip
 Z�    ChartLegend "   LiveCharts.Wpf.DefaultLegend     
BulletSize$ 15Q�#   LiveCharts.Wpf.CartesianChart  -     Name$ StatusBarChart��$��1��$��	0,10,0,40q�   AxisX    LiveCharts.Wpf.Axis     Title$ Статус��   FontSize$ 14Q�   	Separator    LiveCharts.Wpf.Separator     Step$ 1Q�   LabelsRotation$	 -45Q�   AxisY   $ Количество��$ 14Q��� $6��0Статистика по приоритетам��$��20(�$��Medium"�$��0,0,0,20q�  -	   $ PriorityBarChart��$	��300��$��0,0,0,40q�   $ Приоритет��$ 14Q�   $ Количество��$ 14Q��� $9��3Эффективность исполнителей��$��20(�$��Medium"�$��0,0,0,20q�  -
   $ ExecutorsChart��$	��300��$��0,0,0,40q�   $ Исполнитель��$ 14Q�   $% Выполнено заявок��$ 14Q��� $P��JСреднее время выполнения по приоритетам��$��20(�$��Medium"�$��0,0,0,20q�  -   $ CompletionTimeChart��$	��300��$��0,0,0,40q�   $ Приоритет��$ 14Q�   $* $Среднее время (часы)��$ 14Q�