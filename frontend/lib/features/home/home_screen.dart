import 'package:flutter/material.dart';
import 'package:cached_network_image/cached_network_image.dart';
import '../../core/theme/app_theme.dart';

class HomeScreen extends StatelessWidget {
  const HomeScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            Image.asset(
              'assets/images/logo/cavoodle_logo.png',
              height: 40,
              errorBuilder: (context, error, stackTrace) => const Icon(Icons.pets),
            ),
            const SizedBox(width: 8),
            const Text('The Cavoodle Trading Post'),
          ],
        ),
        actions: [
          IconButton(
            icon: const Icon(Icons.favorite_border),
            onPressed: () {
              // TODO: Navigate to watchlist
            },
          ),
          IconButton(
            icon: const Icon(Icons.person_outline),
            onPressed: () {
              // TODO: Navigate to profile/login
            },
          ),
        ],
      ),
      body: CustomScrollView(
        slivers: [
          // Hero banner
          SliverToBoxAdapter(
            child: Container(
              padding: const EdgeInsets.all(24),
              decoration: BoxDecoration(
                gradient: LinearGradient(
                  colors: [
                    AppTheme.caramel.withOpacity(0.8),
                    AppTheme.golden.withOpacity(0.6),
                  ],
                  begin: Alignment.topLeft,
                  end: Alignment.bottomRight,
                ),
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Text(
                    'Find Your Perfect Floof 🐕',
                    style: TextStyle(
                      fontSize: 28,
                      fontWeight: FontWeight.bold,
                      color: Colors.white,
                    ),
                  ),
                  const SizedBox(height: 8),
                  Text(
                    'Where every pup has a story, and that story usually involves eating something they shouldn\'t.',
                    style: TextStyle(
                      fontSize: 14,
                      color: Colors.white.withOpacity(0.9),
                    ),
                  ),
                  const SizedBox(height: 16),
                  // Search bar
                  Container(
                    padding: const EdgeInsets.symmetric(horizontal: 16),
                    decoration: BoxDecoration(
                      color: Colors.white,
                      borderRadius: BorderRadius.circular(30),
                    ),
                    child: Row(
                      children: [
                        const Icon(Icons.search, color: AppTheme.chocolate),
                        const SizedBox(width: 12),
                        Expanded(
                          child: TextField(
                            decoration: const InputDecoration(
                              hintText: 'Search cavoodles...',
                              border: InputBorder.none,
                              filled: false,
                            ),
                            onSubmitted: (value) {
                              // TODO: Search
                            },
                          ),
                        ),
                      ],
                    ),
                  ),
                ],
              ),
            ),
          ),
          
          // Filter chips
          SliverToBoxAdapter(
            child: Padding(
              padding: const EdgeInsets.all(16),
              child: Wrap(
                spacing: 8,
                runSpacing: 8,
                children: [
                  _FilterChip(label: 'All', selected: true, onSelected: (_) {}),
                  _FilterChip(label: 'Cream', selected: false, onSelected: (_) {}),
                  _FilterChip(label: 'Apricot', selected: false, onSelected: (_) {}),
                  _FilterChip(label: 'Chocolate', selected: false, onSelected: (_) {}),
                  _FilterChip(label: 'Puppy', selected: false, onSelected: (_) {}),
                  _FilterChip(label: 'Adult', selected: false, onSelected: (_) {}),
                ],
              ),
            ),
          ),
          
          // Listings grid
          SliverPadding(
            padding: const EdgeInsets.symmetric(horizontal: 16),
            sliver: SliverGrid(
              gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
                crossAxisCount: 2,
                childAspectRatio: 0.7,
                crossAxisSpacing: 12,
                mainAxisSpacing: 12,
              ),
              delegate: SliverChildBuilderDelegate(
                (context, index) => const _ListingCard(),
                childCount: 6, // Placeholder count
              ),
            ),
          ),
          
          const SliverToBoxAdapter(
            child: SizedBox(height: 100),
          ),
        ],
      ),
      floatingActionButton: FloatingActionButton.extended(
        onPressed: () {
          // TODO: Navigate to create listing
        },
        backgroundColor: AppTheme.chocolate,
        icon: const Icon(Icons.add),
        label: const Text('List a Cavoodle'),
      ),
    );
  }
}

class _FilterChip extends StatelessWidget {
  final String label;
  final bool selected;
  final ValueChanged<bool> onSelected;

  const _FilterChip({
    required this.label,
    required this.selected,
    required this.onSelected,
  });

  @override
  Widget build(BuildContext context) {
    return FilterChip(
      label: Text(label),
      selected: selected,
      onSelected: onSelected,
      selectedColor: AppTheme.caramel.withOpacity(0.3),
      checkmarkColor: AppTheme.chocolate,
    );
  }
}

class _ListingCard extends StatelessWidget {
  const _ListingCard();

  @override
  Widget build(BuildContext context) {
    return Card(
      clipBehavior: Clip.antiAlias,
      child: InkWell(
        onTap: () {
          // TODO: Navigate to listing detail
        },
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Image placeholder
            Expanded(
              flex: 3,
              child: Container(
                width: double.infinity,
                color: Colors.grey.shade200,
                child: const Center(
                  child: Icon(
                    Icons.pets,
                    size: 48,
                    color: AppTheme.caramel,
                  ),
                ),
              ),
            ),
            Expanded(
              flex: 2,
              child: Padding(
                padding: const EdgeInsets.all(12),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    const Text(
                      'Fluffy McFluffface',
                      style: TextStyle(
                        fontWeight: FontWeight.bold,
                        fontSize: 14,
                      ),
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                    ),
                    const SizedBox(height: 4),
                    Row(
                      children: [
                        Container(
                          padding: const EdgeInsets.symmetric(
                            horizontal: 6,
                            vertical: 2,
                          ),
                          decoration: BoxDecoration(
                            color: AppTheme.pawPink.withOpacity(0.5),
                            borderRadius: BorderRadius.circular(8),
                          ),
                          child: const Text(
                            '🛋️ Couch Thief',
                            style: TextStyle(fontSize: 10),
                          ),
                        ),
                      ],
                    ),
                    const Spacer(),
                    Row(
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        Text(
                          '\$2,500',
                          style: TextStyle(
                            fontWeight: FontWeight.bold,
                            color: AppTheme.chocolate,
                            fontSize: 16,
                          ),
                        ),
                        Row(
                          children: List.generate(
                            5,
                            (i) => Icon(
                              Icons.cloud,
                              size: 12,
                              color: i < 4 
                                  ? AppTheme.caramel 
                                  : Colors.grey.shade300,
                            ),
                          ),
                        ),
                      ],
                    ),
                  ],
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
