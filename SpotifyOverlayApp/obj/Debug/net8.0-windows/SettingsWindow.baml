   M S B A M L   `   `   `  ����  SpotifyOverlayApp(    "SpotifyOverlayNoAPI.SettingsWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Ayarlar��5   	   $	��300��5   	   $	��350��6    � WindowStartupLocation$ CenterOwner=�5   	    � 
ResizeMode$ NoResize=�5   	    � WindowStyle$
 None=�5   	    � AllowsTransparency$
 True��5	   	   
������  � Topmost$
 True��5   	    � 	Resources 5      3  -System.Windows.Media.Effects.DropShadowEffect   
DropShadow   
BlurRadius   ShadowDepth	   	Direction
   Color �� 
TargetType �� Property �� Value��%   &	        '�� M     '�� �    '�� �      5   
   $ 12Q�6.   $ 2Q�6>   $		 270Q�6N   $
 	#66000000��6^   �� 5   
   " ��6   .�5      ��  � ��$ Segoe UI)�6+   �� 5       � ��$ SemiBold"�6+   �� 5       � ��$	 6,2q�6(   �� 5       � ��$ 4q�6'   �� 5       � �� �	#22FFFFFF6+   �� 5       � ��
 �����6+   �� 5       � �� �	#55FFFFFF6,   �� 5       � ��$ 1q�60   �� 5       �@�Cursor$
 Hand��6'   5      �� 5   
   " ��6   .�5      ��  � ��$ Segoe UI)�6+   �� 5       � ��$ 14(�6)   �� 5       � ��
 �����6+   �� 5       � �� �	#22FFFFFF6+   �� 5       � ��$ 0,4,0,4q�6'   5       �� 5!   
   " ��6   .�5"      ��  � ��$ Segoe UI)�6+   �� 5#       � ��$ 14(�6)   �� 5$       � ��
 �����6+   �� 5%       � �� �	#33FFFFFF6+   �� 5&       � ��$ 0,4,0,4q�6'   �� 5'       � ��$ 4q�6(   5(      5)      .��5+      ��  �� CornerRadius$ 16��6     CurrentGradient#���  5,       J� Effect# [  5-       �� Padding$ 20q�5.      ._�5/   
   � .+�50      �� .+�51      �� $��🎨 Theme Selection��6   $��16(�53      $
��Bold"�54      $��0,0,0,12q�55      
�������56      �� 58      -   5         ��Name$ ThemeComboBox��58      
���   ����	#33000000���	#88FFFFFF$��SemiBold"�5=      .@�5?      �� $
��Blue��6#   
 � Tag$ BluePalette��62   �� 5@      $	��Red��6#   $ 
RedPalette��61   �� 5A      $��Green��6#   $ GreenPalette��63   �� 5B      $��Purple��6#   $ PurplePalette��64   �� 5C      $��Orange��6#   $ OrangePalette��64   �� 5D      $
��Gray��6#   $ GrayPalette��62   �� 5E      $	��RGB��6#   $	 RGB��61   5G      �� 5J      -   5         ��Name$ RgbSpeedLabel��5J      $��RGB Hızı:��63   
�������$
��Bold"�6W   $��0,20,0,4q�6i   $y�	Collapsed=�6{   �� 5K      -   5        $ RgbSpeedComboBox��5K      $y�	Collapsed=�5M      .@�5P      �� 
���   �$��Yavaş��63   $ 60��6C   �� 5Q      
���   �$
��Orta��62   $ 30��6A    �� 
IsSelected$
 True��6J   �� 5R      
���   �$��Hızlı��62   $ 10��6B   5S      �� 5U      $��🔧 Special HEX Code��6   $��14(�5V      $
��Bold"�5W      $��0,20,0,6q�5X      
�������5Y      �� 5\       �� Orientation$ 
Horizontal=�6   .+�5]      �� -   5         ��Name$ HexInput��5]      $	��120��60   $��	#FF007ACC��6<   �� 5^      -   5        $��Apply��5^        ModernButtonStyle#��[ 5_   	   $��4q�64   5a      �� 5c      -   5        $��Close��5c      #��[ 5d   	   $��Right=�64   $��4q�6P   5e      5f      5g      5h      