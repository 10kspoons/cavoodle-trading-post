import 'package:flutter/material.dart';

class ListingDetailScreen extends StatelessWidget {
  final String listingId;

  const ListingDetailScreen({super.key, required this.listingId});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Cavoodle Details'),
      ),
      body: const Center(
        child: Text('Listing Detail - Coming Soon'),
      ),
    );
  }
}
