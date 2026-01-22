import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';
import '../../core/theme/app_theme.dart';

class CreateListingScreen extends StatefulWidget {
  const CreateListingScreen({super.key});

  @override
  State<CreateListingScreen> createState() => _CreateListingScreenState();
}

class _CreateListingScreenState extends State<CreateListingScreen> {
  final _formKey = GlobalKey<FormState>();
  final _nameController = TextEditingController();
  final _priceController = TextEditingController();
  final _descriptionController = TextEditingController();
  final _suburbController = TextEditingController();
  
  int _ageMonths = 3;
  String _color = 'Cream';
  String _gender = 'Male';
  String _state = 'NSW';
  final List<XFile> _photos = [];
  bool _isLoading = false;

  final _colors = ['Cream', 'Apricot', 'Gold', 'Red', 'Chocolate', 'Black', 'Phantom', 'Parti', 'Merle'];
  final _states = ['NSW', 'VIC', 'QLD', 'WA', 'SA', 'TAS', 'ACT', 'NT'];

  @override
  void dispose() {
    _nameController.dispose();
    _priceController.dispose();
    _descriptionController.dispose();
    _suburbController.dispose();
    super.dispose();
  }

  Future<void> _pickImage() async {
    if (_photos.length >= 6) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Maximum 6 photos allowed')),
      );
      return;
    }

    final picker = ImagePicker();
    final image = await picker.pickImage(source: ImageSource.gallery);
    if (image != null) {
      setState(() => _photos.add(image));
    }
  }

  void _removePhoto(int index) {
    setState(() => _photos.removeAt(index));
  }

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) return;
    if (_photos.isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Please add at least one photo')),
      );
      return;
    }

    setState(() => _isLoading = true);

    try {
      // TODO: Upload photos to MinIO first, then create listing
      await Future.delayed(const Duration(seconds: 1)); // Placeholder
      
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Listing created! 🐕'),
            backgroundColor: Colors.green,
          ),
        );
        Navigator.pop(context);
      }
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Failed to create listing')),
      );
    } finally {
      if (mounted) {
        setState(() => _isLoading = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('List Your Cavoodle'),
      ),
      body: Form(
        key: _formKey,
        child: ListView(
          padding: const EdgeInsets.all(16),
          children: [
            // Photos section
            Text(
              'Photos',
              style: Theme.of(context).textTheme.titleMedium,
            ),
            const SizedBox(height: 8),
            SizedBox(
              height: 120,
              child: ListView(
                scrollDirection: Axis.horizontal,
                children: [
                  ..._photos.asMap().entries.map((entry) {
                    return Padding(
                      padding: const EdgeInsets.only(right: 8),
                      child: Stack(
                        children: [
                          ClipRRect(
                            borderRadius: BorderRadius.circular(12),
                            child: Image.network(
                              entry.value.path,
                              width: 100,
                              height: 100,
                              fit: BoxFit.cover,
                              errorBuilder: (_, __, ___) => Container(
                                width: 100,
                                height: 100,
                                color: Colors.grey[200],
                                child: const Icon(Icons.image),
                              ),
                            ),
                          ),
                          Positioned(
                            top: 4,
                            right: 4,
                            child: GestureDetector(
                              onTap: () => _removePhoto(entry.key),
                              child: Container(
                                padding: const EdgeInsets.all(4),
                                decoration: const BoxDecoration(
                                  color: Colors.red,
                                  shape: BoxShape.circle,
                                ),
                                child: const Icon(
                                  Icons.close,
                                  size: 16,
                                  color: Colors.white,
                                ),
                              ),
                            ),
                          ),
                          if (entry.key == 0)
                            Positioned(
                              bottom: 4,
                              left: 4,
                              child: Container(
                                padding: const EdgeInsets.symmetric(
                                  horizontal: 6,
                                  vertical: 2,
                                ),
                                decoration: BoxDecoration(
                                  color: AppTheme.caramel,
                                  borderRadius: BorderRadius.circular(8),
                                ),
                                child: const Text(
                                  'Primary',
                                  style: TextStyle(
                                    color: Colors.white,
                                    fontSize: 10,
                                  ),
                                ),
                              ),
                            ),
                        ],
                      ),
                    );
                  }),
                  if (_photos.length < 6)
                    GestureDetector(
                      onTap: _pickImage,
                      child: Container(
                        width: 100,
                        height: 100,
                        decoration: BoxDecoration(
                          color: Colors.grey[100],
                          borderRadius: BorderRadius.circular(12),
                          border: Border.all(color: Colors.grey[300]!),
                        ),
                        child: Column(
                          mainAxisAlignment: MainAxisAlignment.center,
                          children: [
                            Icon(Icons.add_a_photo, color: Colors.grey[600]),
                            const SizedBox(height: 4),
                            Text(
                              'Add Photo',
                              style: TextStyle(
                                fontSize: 12,
                                color: Colors.grey[600],
                              ),
                            ),
                          ],
                        ),
                      ),
                    ),
                ],
              ),
            ),
            const SizedBox(height: 24),

            // Name
            TextFormField(
              controller: _nameController,
              decoration: const InputDecoration(
                labelText: 'Cavoodle Name *',
                hintText: 'e.g., Fluffy McFluffface',
                prefixIcon: Icon(Icons.pets),
              ),
              validator: (v) => v?.isEmpty == true ? 'Name is required' : null,
            ),
            const SizedBox(height: 16),

            // Price
            TextFormField(
              controller: _priceController,
              keyboardType: TextInputType.number,
              decoration: const InputDecoration(
                labelText: 'Price *',
                hintText: 'e.g., 2500',
                prefixIcon: Icon(Icons.attach_money),
              ),
              validator: (v) {
                if (v?.isEmpty == true) return 'Price is required';
                if (double.tryParse(v!) == null) return 'Invalid price';
                return null;
              },
            ),
            const SizedBox(height: 16),

            // Age slider
            Text(
              'Age: ${_ageMonths < 12 ? '$_ageMonths months' : '${_ageMonths ~/ 12} year${_ageMonths >= 24 ? 's' : ''}'}',
              style: Theme.of(context).textTheme.bodyMedium,
            ),
            Slider(
              value: _ageMonths.toDouble(),
              min: 1,
              max: 120,
              divisions: 119,
              activeColor: AppTheme.caramel,
              onChanged: (v) => setState(() => _ageMonths = v.round()),
            ),
            const SizedBox(height: 16),

            // Color & Gender row
            Row(
              children: [
                Expanded(
                  child: DropdownButtonFormField<String>(
                    value: _color,
                    decoration: const InputDecoration(labelText: 'Color'),
                    items: _colors.map((c) => DropdownMenuItem(
                      value: c,
                      child: Text(c),
                    )).toList(),
                    onChanged: (v) => setState(() => _color = v!),
                  ),
                ),
                const SizedBox(width: 16),
                Expanded(
                  child: DropdownButtonFormField<String>(
                    value: _gender,
                    decoration: const InputDecoration(labelText: 'Gender'),
                    items: ['Male', 'Female'].map((g) => DropdownMenuItem(
                      value: g,
                      child: Row(
                        children: [
                          Icon(
                            g == 'Male' ? Icons.male : Icons.female,
                            size: 18,
                          ),
                          const SizedBox(width: 4),
                          Text(g),
                        ],
                      ),
                    )).toList(),
                    onChanged: (v) => setState(() => _gender = v!),
                  ),
                ),
              ],
            ),
            const SizedBox(height: 16),

            // Location row
            Row(
              children: [
                Expanded(
                  flex: 2,
                  child: TextFormField(
                    controller: _suburbController,
                    decoration: const InputDecoration(
                      labelText: 'Suburb *',
                      prefixIcon: Icon(Icons.location_on),
                    ),
                    validator: (v) => v?.isEmpty == true ? 'Suburb required' : null,
                  ),
                ),
                const SizedBox(width: 16),
                Expanded(
                  child: DropdownButtonFormField<String>(
                    value: _state,
                    decoration: const InputDecoration(labelText: 'State'),
                    items: _states.map((s) => DropdownMenuItem(
                      value: s,
                      child: Text(s),
                    )).toList(),
                    onChanged: (v) => setState(() => _state = v!),
                  ),
                ),
              ],
            ),
            const SizedBox(height: 16),

            // Description
            TextFormField(
              controller: _descriptionController,
              maxLines: 4,
              decoration: const InputDecoration(
                labelText: 'Description *',
                hintText: 'Tell us about your cavoodle...',
                alignLabelWithHint: true,
              ),
              validator: (v) => v?.isEmpty == true ? 'Description required' : null,
            ),
            const SizedBox(height: 32),

            // Submit
            SizedBox(
              height: 50,
              child: ElevatedButton.icon(
                onPressed: _isLoading ? null : _submit,
                icon: _isLoading
                    ? const SizedBox(
                        width: 20,
                        height: 20,
                        child: CircularProgressIndicator(strokeWidth: 2),
                      )
                    : const Icon(Icons.check),
                label: Text(_isLoading ? 'Creating...' : 'Create Listing'),
              ),
            ),
            const SizedBox(height: 40),
          ],
        ),
      ),
    );
  }
}
