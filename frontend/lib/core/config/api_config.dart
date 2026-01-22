import 'package:flutter/foundation.dart';

class ApiConfig {
  static String get baseUrl {
    if (kIsWeb) {
      // Web: Use relative URLs (same domain)
      return '/api';
    } else {
      // Mobile: Use full URL
      return const String.fromEnvironment(
        'API_URL',
        defaultValue: 'https://cavoodle-trading-post.com.au/api',
      );
    }
  }
  
  static String get wsUrl {
    if (kIsWeb) {
      final uri = Uri.base;
      final protocol = uri.scheme == 'https' ? 'wss' : 'ws';
      return '$protocol://${uri.host}:${uri.port}/hubs/messaging';
    } else {
      return const String.fromEnvironment(
        'WS_URL',
        defaultValue: 'wss://cavoodle-trading-post.com.au/hubs/messaging',
      );
    }
  }
}
