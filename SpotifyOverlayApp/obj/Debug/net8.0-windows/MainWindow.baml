   M S B A M L   `   `   `  ����  SpotifyOverlayApp$    SpotifyOverlayNoAPI.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  SpotifyOverlay��5   	   $	��420��5   	   $	��150��6    � WindowStyle$
 None=�5   	    � AllowsTransparency$
 True��5   	   
������  � Topmost$
 True��5	   	    � ShowInTaskbar$ False��5
   	    J� Opacity$ 0Q�5   	    � 	Resources 5      .  (System.Windows.Media.Animation.CubicEase   
SmoothEase7  1System.Windows.Media.Animation.EasingFunctionBase   
EasingMode 
 FadeIn �� TargetProperty	 W� From	
 W� To g� Duration W� EasingFunction  FadeOut  PausedShrink �� 
TargetName  PlayingGrow  VolumeBarBrush �� 
StartPoint �� EndPoint � Color � Offset  ModernButtonStyle �� 
TargetType �� Property �� Value �� 
TargetType �� CornerRadius��%�  &	        &	 #     7   &	 �     7   &	     &	 g    &	 �    &	 �      5   
   $ 	EaseInOut=�6'   �� 5   
   .��5      W� $ Opacity�6   	 05      
 16'   $ 0:0:0.7D�6.   8   5      5      �� 5   
   .��5      W� $ Opacity�6   	 15      
 06'   $ 0:0:0.7D�6.   8   5      5      �� 5   
   .��5      W� $ 
MainBorder��6   $ LayoutTransform.ScaleX�5      
 0.925       $ 0:0:0.3D�6(   W� 5!      $ 
MainBorder��6   $ LayoutTransform.ScaleY�5"      
 0.925#      $ 0:0:0.3D�6(   W� 5$      $ 
MainBorder��6   $ Opacity�5%      
 0.655&      $ 0:0:0.3D�6(   5'      �� 5)   
   .��5*      W� $ 
MainBorder��6   $ LayoutTransform.ScaleX�5+      
 1.05,      $ 0:0:0.3D�6'   W� 5-      $ 
MainBorder��6   $ LayoutTransform.ScaleY�5.      
 1.05/      $ 0:0:0.3D�6'   W� 50      $ 
MainBorder��6   $ Opacity�51      
 1.052      $ 0:0:0.3D�6'   53      �� 56   
   $	 0,0&�65   $	 1,0&�6F   .��57      � $ 	LimeGreen��6   $	 0.0Q�6-   � 58      $ Yellow��6   $	 0.5Q�6*   � 59      $	 Red��6   $	 1.0Q�6'   5:      �� 5=   
   " ��6*   .�5>      ��  � �� �#1E1E1E6+   �� 5?       � ��
 �����6+   �� 5@       � ��$ 5q�6(   �� 5A       �@�Cursor$
 Hand��6'   �� 5B       � ��$
 Bold"�6+   �� 5C       � ��$ 0q�60   �� 5E       � �� 5F      �� 5G      " ��6&   .R�5H      �� #��z��6!   $ 8��5I   !   ._�5J      �� $��Center=�6/   $��Center=�6L   5K      5L      5N      5O      5P      .��5R      � $��10q�6   .+�5S   
   �� -   5         ��Name$ 
MainBorder��5S        CurrentGradient#���  5T      $ 18��5U       �� Padding$ 18q�5V        
DropShadow J� Effect# �  5W       J� IsHitTestVisible$ False��5X       � LayoutTransform 5Y      �� 5Z       �� ScaleX$ 1Q�6!    �� ScaleY$ 1Q�6,   ._�5]      �� .+�5^      �� -   5         ��Name$ SongText��5^      $��20(�5_      
�������$
��Bold"�5a      $��Segoe UI)�5c      $��CharacterEllipsis=�5d      $��NoWrap=�5e      $��0,0,0,6q�5f      �� 5h      -   5        $ 
VolumeText��5h      $��13(�5i      ���#CCCCCC$��Segoe UI Semilight)�5k      $��NoWrap=�5l      $��0,0,0,2q�5m      � 5o      -   5         �Name$ 	VolumeBar��5o      $��6��5p        � Minimum$  0Q�5q      ! � Maximum$	! 100Q�6   " � Value$" 0Q�5r      $��0,8,0,0q�5s      #���  5t      ���#222$��0q�5v      5z      5{      �� 5}   
   -   5        # ��Name$# SettingsButton��5}      $	��⚙��5~      $��30��5      $��30��6   $��Right=�5�      $��Bottom=�5�      $��10q�5�      #��[ 5�      $��16(�5�      5�      5�      