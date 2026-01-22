import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'core/theme/app_theme.dart';
import 'routing/app_router.dart';

class CavoodleTradingApp extends StatelessWidget {
  const CavoodleTradingApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp.router(
      title: 'The Cavoodle Trading Post',
      debugShowCheckedModeBanner: false,
      theme: AppTheme.lightTheme,
      darkTheme: AppTheme.darkTheme,
      themeMode: ThemeMode.system,
      routerConfig: appRouter,
    );
  }
}
