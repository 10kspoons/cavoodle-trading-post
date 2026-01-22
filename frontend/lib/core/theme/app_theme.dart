import 'package:flutter/material.dart';

class AppTheme {
  // Cavoodle-inspired color palette
  static const Color cream = Color(0xFFFFF8E7);
  static const Color caramel = Color(0xFFD4A574);
  static const Color chocolate = Color(0xFF5D4037);
  static const Color charcoal = Color(0xFF424242);
  static const Color golden = Color(0xFFFFB74D);
  static const Color pawPink = Color(0xFFFFCDD2);
  static const Color mintGreen = Color(0xFFB2DFDB);

  static ThemeData get lightTheme {
    return ThemeData(
      useMaterial3: true,
      brightness: Brightness.light,
      colorScheme: ColorScheme.fromSeed(
        seedColor: caramel,
        brightness: Brightness.light,
        primary: caramel,
        secondary: chocolate,
        surface: cream,
        background: Colors.white,
      ),
      scaffoldBackgroundColor: cream,
      appBarTheme: const AppBarTheme(
        backgroundColor: caramel,
        foregroundColor: Colors.white,
        elevation: 0,
        centerTitle: true,
      ),
      cardTheme: CardTheme(
        elevation: 2,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(16),
        ),
        color: Colors.white,
      ),
      elevatedButtonTheme: ElevatedButtonThemeData(
        style: ElevatedButton.styleFrom(
          backgroundColor: caramel,
          foregroundColor: Colors.white,
          padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 12),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(24),
          ),
        ),
      ),
      outlinedButtonTheme: OutlinedButtonThemeData(
        style: OutlinedButton.styleFrom(
          foregroundColor: chocolate,
          side: const BorderSide(color: chocolate),
          padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 12),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(24),
          ),
        ),
      ),
      inputDecorationTheme: InputDecorationTheme(
        filled: true,
        fillColor: Colors.white,
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
          borderSide: BorderSide.none,
        ),
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
          borderSide: BorderSide(color: Colors.grey.shade300),
        ),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
          borderSide: const BorderSide(color: caramel, width: 2),
        ),
        contentPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
      ),
      chipTheme: ChipThemeData(
        backgroundColor: pawPink.withOpacity(0.3),
        labelStyle: const TextStyle(color: chocolate),
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(20),
        ),
      ),
      fontFamily: 'Nunito',
      textTheme: const TextTheme(
        displayLarge: TextStyle(
          fontSize: 32,
          fontWeight: FontWeight.bold,
          color: chocolate,
        ),
        headlineMedium: TextStyle(
          fontSize: 24,
          fontWeight: FontWeight.w600,
          color: chocolate,
        ),
        titleLarge: TextStyle(
          fontSize: 20,
          fontWeight: FontWeight.w600,
          color: charcoal,
        ),
        bodyLarge: TextStyle(
          fontSize: 16,
          color: charcoal,
        ),
        bodyMedium: TextStyle(
          fontSize: 14,
          color: charcoal,
        ),
      ),
    );
  }

  static ThemeData get darkTheme {
    return ThemeData(
      useMaterial3: true,
      brightness: Brightness.dark,
      colorScheme: ColorScheme.fromSeed(
        seedColor: caramel,
        brightness: Brightness.dark,
        primary: golden,
        secondary: caramel,
      ),
      scaffoldBackgroundColor: const Color(0xFF1A1A1A),
      appBarTheme: const AppBarTheme(
        backgroundColor: Color(0xFF2D2D2D),
        foregroundColor: Colors.white,
        elevation: 0,
        centerTitle: true,
      ),
      cardTheme: CardTheme(
        elevation: 2,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(16),
        ),
        color: const Color(0xFF2D2D2D),
      ),
      elevatedButtonTheme: ElevatedButtonThemeData(
        style: ElevatedButton.styleFrom(
          backgroundColor: golden,
          foregroundColor: charcoal,
          padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 12),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(24),
          ),
        ),
      ),
      fontFamily: 'Nunito',
    );
  }
}
