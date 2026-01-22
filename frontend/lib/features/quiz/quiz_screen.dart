import 'package:flutter/material.dart';

class QuizScreen extends StatelessWidget {
  final String listingId;

  const QuizScreen({super.key, required this.listingId});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Personality Quiz'),
      ),
      body: const Center(
        child: Text('Quiz Screen - Coming Soon'),
      ),
    );
  }
}
