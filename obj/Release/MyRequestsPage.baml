   M S B A M L   `   `   `  ����   
ServiceWPF    ServiceWPF.MyRequestsPage   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       N� Title$  Мои заявки�� N� 
Background
 �����.��� $��0q� � RowDefinitions �� $
��Auto ��� .+�� $��0,0,0,20q� � ColumnDefinitions �� �� $
��Auto �.+��� 
�������$��1q����#E0E0E0 �� CornerRadius$ 4��$��0,0,10,0q�._��  �� $
��Auto ��� .+��� $
��🔍��$
��10,0q�$��Center=����#757575�� -    ��Name$ 	SearchBox��$��1��$��0q�$	��0,8q�
������ $��0,0,10,0q�$��Center=��� $��1�� �� Orientation$ 
Horizontal=�$��10,0,0,0q�.+��� -    ��Name$ StatusFilter��$	��150��$��0,0,10,0q�
�������.@��� $��Все статусы��
��������� $��
Новая��
��������� $��В работе��
��������� $��Завершена��
��������� $��Отменена��
��������� -   $ 
SortFilter��$	��150��
�������.@��� $��Сначала новые��
��������� $!��Сначала старые��
��������� $��По приоритету��
��������� $��1��$��1q����#E0E0E0
�������$ 4��._��� $
��Auto=�$��Disabled=�$��0,0,8,0q�.���� -    ��Name$ RequestsList��$��0q�
�������$��Disabled=�$��Disabled=����� .R��� ���� 	 �� 
TargetType"	 ��.��� 
 �� Property
 � �� �� Value$ Stretch=��� 
 � ��$ Stretch=��� 
 � ��$ 0q��� 
 � ��
 ������� 
 � ��$ 0q� �� Triggers P�  P� Property �@��
IsSelected P� Value . . ��� 
 � ��
 �����P�  �@J�IsMouseOver . . ��� 
 � �� �#F5F5F5���� .R���  �� Padding$ 15q�
�������$��0,0,0,1q����#E0E0E0$��Stretch=�._�� $��Stretch=� �� $��* ��� $
��Auto �.+��� $��Stretch=�.+��� $��16(�$��Medium"����� *Title+�� $	��0,5q�$
��Wrap=����#757575���� *Description+�� $ 
Horizontal=�$��0,5,0,0q�.+��� ���#757575���� *CreatedDate+�� $�� • �����#757575�� ���#2196F3���� *Status+�� $��1��$��20,0,0,0q����#FFF3E0$	 8,4q�$ 4��$	��Top=�._��� ���#F57C00���� *
Priority+