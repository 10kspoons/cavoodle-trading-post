import 'package:flutter/material.dart';
import 'package:cached_network_image/cached_network_image.dart';
import '../../core/theme/app_theme.dart';
import '../../core/services/api_service.dart';

class ListingDetailScreen extends StatefulWidget {
  final String listingId;

  const ListingDetailScreen({super.key, required this.listingId});

  @override
  State<ListingDetailScreen> createState() => _ListingDetailScreenState();
}

class _ListingDetailScreenState extends State<ListingDetailScreen> {
  ListingDetail? _listing;
  bool _isLoading = true;
  String? _error;
  int _currentImageIndex = 0;

  @override
  void initState() {
    super.initState();
    _loadListing();
  }

  Future<void> _loadListing() async {
    try {
      final listing = await ApiService().getListing(widget.listingId);
      setState(() {
        _listing = listing;
        _isLoading = false;
      });
    } catch (e) {
      setState(() {
        _error = 'Failed to load listing';
        _isLoading = false;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    if (_isLoading) {
      return Scaffold(
        appBar: AppBar(),
        body: const Center(child: CircularProgressIndicator()),
      );
    }

    if (_error != null || _listing == null) {
      return Scaffold(
        appBar: AppBar(),
        body: Center(
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              const Icon(Icons.error_outline, size: 48, color: Colors.grey),
              const SizedBox(height: 16),
              Text(_error ?? 'Listing not found'),
              const SizedBox(height: 16),
              ElevatedButton(
                onPressed: () => Navigator.pop(context),
                child: const Text('Go Back'),
              ),
            ],
          ),
        ),
      );
    }

    final listing = _listing!;

    return Scaffold(
      body: CustomScrollView(
        slivers: [
          // Image carousel app bar
          SliverAppBar(
            expandedHeight: 300,
            pinned: true,
            flexibleSpace: FlexibleSpaceBar(
              background: Stack(
                fit: StackFit.expand,
                children: [
                  if (listing.photos.isNotEmpty)
                    PageView.builder(
                      itemCount: listing.photos.length,
                      onPageChanged: (i) => setState(() => _currentImageIndex = i),
                      itemBuilder: (context, index) {
                        return CachedNetworkImage(
                          imageUrl: listing.photos[index].url,
                          fit: BoxFit.cover,
                          placeholder: (_, __) => Container(
                            color: Colors.grey[200],
                            child: const Center(child: CircularProgressIndicator()),
                          ),
                          errorWidget: (_, __, ___) => Container(
                            color: Colors.grey[200],
                            child: const Icon(Icons.pets, size: 64, color: AppTheme.caramel),
                          ),
                        );
                      },
                    )
                  else
                    Container(
                      color: Colors.grey[200],
                      child: const Icon(Icons.pets, size: 64, color: AppTheme.caramel),
                    ),
                  // Image indicators
                  if (listing.photos.length > 1)
                    Positioned(
                      bottom: 16,
                      left: 0,
                      right: 0,
                      child: Row(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: List.generate(
                          listing.photos.length,
                          (i) => Container(
                            width: 8,
                            height: 8,
                            margin: const EdgeInsets.symmetric(horizontal: 4),
                            decoration: BoxDecoration(
                              shape: BoxShape.circle,
                              color: i == _currentImageIndex
                                  ? Colors.white
                                  : Colors.white.withOpacity(0.5),
                            ),
                          ),
                        ),
                      ),
                    ),
                ],
              ),
            ),
            actions: [
              IconButton(
                icon: const Icon(Icons.favorite_border),
                onPressed: () {
                  // TODO: Add to watchlist
                },
              ),
              IconButton(
                icon: const Icon(Icons.share),
                onPressed: () {
                  // TODO: Share listing
                },
              ),
            ],
          ),

          // Content
          SliverToBoxAdapter(
            child: Padding(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  // Name and price
                  Row(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Expanded(
                        child: Text(
                          listing.name,
                          style: Theme.of(context).textTheme.headlineMedium,
                        ),
                      ),
                      Text(
                        '\$${listing.price.toStringAsFixed(0)}',
                        style: Theme.of(context).textTheme.headlineMedium?.copyWith(
                          color: AppTheme.chocolate,
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 8),

                  // Location and age
                  Wrap(
                    spacing: 16,
                    runSpacing: 8,
                    children: [
                      _InfoChip(icon: Icons.location_on, label: '${listing.suburb}, ${listing.state}'),
                      _InfoChip(icon: Icons.cake, label: _formatAge(listing.ageMonths)),
                      _InfoChip(icon: Icons.palette, label: listing.color),
                      _InfoChip(
                        icon: listing.gender == 'Male' ? Icons.male : Icons.female,
                        label: listing.gender,
                      ),
                    ],
                  ),
                  const SizedBox(height: 24),

                  // Personality section
                  if (listing.personality != null) ...[
                    Container(
                      padding: const EdgeInsets.all(16),
                      decoration: BoxDecoration(
                        gradient: LinearGradient(
                          colors: [
                            AppTheme.pawPink.withOpacity(0.3),
                            AppTheme.mintGreen.withOpacity(0.3),
                          ],
                        ),
                        borderRadius: BorderRadius.circular(16),
                      ),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Row(
                            children: [
                              const Text('🎭', style: TextStyle(fontSize: 24)),
                              const SizedBox(width: 8),
                              Text(
                                listing.personality!.typeDisplayName,
                                style: Theme.of(context).textTheme.titleLarge?.copyWith(
                                  fontWeight: FontWeight.bold,
                                ),
                              ),
                            ],
                          ),
                          if (listing.personality!.customBio != null) ...[
                            const SizedBox(height: 8),
                            Text(
                              listing.personality!.customBio!,
                              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                                fontStyle: FontStyle.italic,
                              ),
                            ),
                          ],
                          const SizedBox(height: 16),
                          Row(
                            mainAxisAlignment: MainAxisAlignment.spaceAround,
                            children: [
                              _RatingBar(label: 'Energy', value: listing.personality!.energyLevel, icon: '⚡'),
                              _RatingBar(label: 'Floof', value: listing.personality!.floofFactor, icon: '☁️'),
                              _RatingBar(label: 'Sass', value: listing.personality!.sassRating, icon: '😏'),
                            ],
                          ),
                        ],
                      ),
                    ),
                    const SizedBox(height: 24),
                  ],

                  // Description
                  Text(
                    'About ${listing.name}',
                    style: Theme.of(context).textTheme.titleLarge,
                  ),
                  const SizedBox(height: 8),
                  Text(
                    listing.description,
                    style: Theme.of(context).textTheme.bodyLarge,
                  ),
                  const SizedBox(height: 24),

                  // Seller card
                  Container(
                    padding: const EdgeInsets.all(16),
                    decoration: BoxDecoration(
                      color: Colors.white,
                      borderRadius: BorderRadius.circular(12),
                      boxShadow: [
                        BoxShadow(
                          color: Colors.black.withOpacity(0.05),
                          blurRadius: 10,
                        ),
                      ],
                    ),
                    child: Row(
                      children: [
                        CircleAvatar(
                          radius: 28,
                          backgroundColor: AppTheme.caramel,
                          backgroundImage: listing.seller.avatarUrl != null
                              ? CachedNetworkImageProvider(listing.seller.avatarUrl!)
                              : null,
                          child: listing.seller.avatarUrl == null
                              ? Text(
                                  listing.seller.displayName[0].toUpperCase(),
                                  style: const TextStyle(
                                    color: Colors.white,
                                    fontSize: 20,
                                    fontWeight: FontWeight.bold,
                                  ),
                                )
                              : null,
                        ),
                        const SizedBox(width: 12),
                        Expanded(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(
                                listing.seller.displayName,
                                style: Theme.of(context).textTheme.titleMedium,
                              ),
                              Row(
                                children: [
                                  const Icon(Icons.star, size: 16, color: AppTheme.golden),
                                  const SizedBox(width: 4),
                                  Text(
                                    '${listing.seller.cavoodleKarma} Karma',
                                    style: Theme.of(context).textTheme.bodySmall,
                                  ),
                                ],
                              ),
                            ],
                          ),
                        ),
                        OutlinedButton(
                          onPressed: () {
                            // TODO: View seller profile
                          },
                          child: const Text('View Profile'),
                        ),
                      ],
                    ),
                  ),
                  const SizedBox(height: 100), // Space for bottom bar
                ],
              ),
            ),
          ),
        ],
      ),
      bottomNavigationBar: Container(
        padding: const EdgeInsets.all(16),
        decoration: BoxDecoration(
          color: Colors.white,
          boxShadow: [
            BoxShadow(
              color: Colors.black.withOpacity(0.1),
              blurRadius: 10,
              offset: const Offset(0, -2),
            ),
          ],
        ),
        child: SafeArea(
          child: Row(
            children: [
              Expanded(
                child: ElevatedButton.icon(
                  onPressed: () {
                    // TODO: Start conversation
                  },
                  icon: const Icon(Icons.chat_bubble_outline),
                  label: const Text('Message Seller'),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  String _formatAge(int months) {
    if (months < 12) {
      return '$months month${months == 1 ? '' : 's'}';
    }
    final years = months ~/ 12;
    final remainingMonths = months % 12;
    if (remainingMonths == 0) {
      return '$years year${years == 1 ? '' : 's'}';
    }
    return '$years year${years == 1 ? '' : 's'}, $remainingMonths month${remainingMonths == 1 ? '' : 's'}';
  }
}

class _InfoChip extends StatelessWidget {
  final IconData icon;
  final String label;

  const _InfoChip({required this.icon, required this.label});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
      decoration: BoxDecoration(
        color: Colors.grey[100],
        borderRadius: BorderRadius.circular(20),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(icon, size: 16, color: AppTheme.chocolate),
          const SizedBox(width: 4),
          Text(label, style: const TextStyle(fontSize: 13)),
        ],
      ),
    );
  }
}

class _RatingBar extends StatelessWidget {
  final String label;
  final int value;
  final String icon;

  const _RatingBar({required this.label, required this.value, required this.icon});

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Text(icon, style: const TextStyle(fontSize: 20)),
        const SizedBox(height: 4),
        Text(label, style: const TextStyle(fontSize: 12, fontWeight: FontWeight.w500)),
        const SizedBox(height: 4),
        Row(
          mainAxisSize: MainAxisSize.min,
          children: List.generate(5, (i) {
            return Icon(
              Icons.circle,
              size: 10,
              color: i < value ? AppTheme.caramel : Colors.grey[300],
            );
          }),
        ),
      ],
    );
  }
}
