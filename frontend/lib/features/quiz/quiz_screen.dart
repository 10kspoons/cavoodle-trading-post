import 'package:flutter/material.dart';
import '../../core/theme/app_theme.dart';
import '../../core/services/api_service.dart';

class QuizScreen extends StatefulWidget {
  final String listingId;

  const QuizScreen({super.key, required this.listingId});

  @override
  State<QuizScreen> createState() => _QuizScreenState();
}

class _QuizScreenState extends State<QuizScreen> {
  List<QuizQuestion> _questions = [];
  bool _isLoading = true;
  bool _isSubmitting = false;
  int _currentQuestionIndex = 0;
  final Map<int, String> _answers = {};
  PersonalityResult? _result;

  @override
  void initState() {
    super.initState();
    _loadQuestions();
  }

  Future<void> _loadQuestions() async {
    try {
      final questions = await ApiService().getQuizQuestions();
      setState(() {
        _questions = questions;
        _isLoading = false;
      });
    } catch (e) {
      setState(() => _isLoading = false);
    }
  }

  void _selectAnswer(String optionId) {
    setState(() {
      _answers[_questions[_currentQuestionIndex].id] = optionId;
    });
  }

  void _nextQuestion() {
    if (_currentQuestionIndex < _questions.length - 1) {
      setState(() => _currentQuestionIndex++);
    } else {
      _submitQuiz();
    }
  }

  void _previousQuestion() {
    if (_currentQuestionIndex > 0) {
      setState(() => _currentQuestionIndex--);
    }
  }

  Future<void> _submitQuiz() async {
    setState(() => _isSubmitting = true);
    try {
      final result = await ApiService().calculatePersonality(_answers);
      setState(() {
        _result = result;
        _isSubmitting = false;
      });
    } catch (e) {
      setState(() => _isSubmitting = false);
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Failed to calculate personality')),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    if (_isLoading) {
      return Scaffold(
        appBar: AppBar(title: const Text('Personality Quiz')),
        body: const Center(child: CircularProgressIndicator()),
      );
    }

    if (_result != null) {
      return _ResultScreen(result: _result!, listingId: widget.listingId);
    }

    if (_questions.isEmpty) {
      return Scaffold(
        appBar: AppBar(title: const Text('Personality Quiz')),
        body: const Center(child: Text('No questions available')),
      );
    }

    final question = _questions[_currentQuestionIndex];
    final selectedAnswer = _answers[question.id];

    return Scaffold(
      appBar: AppBar(
        title: const Text('Personality Quiz'),
        bottom: PreferredSize(
          preferredSize: const Size.fromHeight(4),
          child: LinearProgressIndicator(
            value: (_currentQuestionIndex + 1) / _questions.length,
            backgroundColor: Colors.grey[300],
            valueColor: const AlwaysStoppedAnimation<Color>(AppTheme.caramel),
          ),
        ),
      ),
      body: Padding(
        padding: const EdgeInsets.all(20),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            // Question number
            Text(
              'Question ${_currentQuestionIndex + 1} of ${_questions.length}',
              style: TextStyle(color: Colors.grey[600]),
            ),
            const SizedBox(height: 8),

            // Question
            Text(
              question.question,
              style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 32),

            // Options
            Expanded(
              child: ListView.separated(
                itemCount: question.options.length,
                separatorBuilder: (_, __) => const SizedBox(height: 12),
                itemBuilder: (context, index) {
                  final option = question.options[index];
                  final isSelected = selectedAnswer == option.id;
                  
                  return _OptionCard(
                    option: option,
                    isSelected: isSelected,
                    onTap: () => _selectAnswer(option.id),
                  );
                },
              ),
            ),

            // Navigation
            Row(
              children: [
                if (_currentQuestionIndex > 0)
                  Expanded(
                    child: OutlinedButton(
                      onPressed: _previousQuestion,
                      child: const Text('Previous'),
                    ),
                  ),
                if (_currentQuestionIndex > 0) const SizedBox(width: 12),
                Expanded(
                  flex: 2,
                  child: ElevatedButton(
                    onPressed: selectedAnswer != null
                        ? (_isSubmitting ? null : _nextQuestion)
                        : null,
                    child: _isSubmitting
                        ? const SizedBox(
                            width: 20,
                            height: 20,
                            child: CircularProgressIndicator(
                              strokeWidth: 2,
                              color: Colors.white,
                            ),
                          )
                        : Text(
                            _currentQuestionIndex == _questions.length - 1
                                ? 'See Results 🎉'
                                : 'Next',
                          ),
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}

class _OptionCard extends StatelessWidget {
  final QuizOption option;
  final bool isSelected;
  final VoidCallback onTap;

  const _OptionCard({
    required this.option,
    required this.isSelected,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return AnimatedContainer(
      duration: const Duration(milliseconds: 200),
      decoration: BoxDecoration(
        color: isSelected ? AppTheme.caramel.withOpacity(0.1) : Colors.white,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(
          color: isSelected ? AppTheme.caramel : Colors.grey[300]!,
          width: isSelected ? 2 : 1,
        ),
      ),
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(12),
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: Row(
            children: [
              Container(
                width: 28,
                height: 28,
                decoration: BoxDecoration(
                  shape: BoxShape.circle,
                  color: isSelected ? AppTheme.caramel : Colors.grey[200],
                ),
                child: Center(
                  child: Text(
                    option.id,
                    style: TextStyle(
                      fontWeight: FontWeight.bold,
                      color: isSelected ? Colors.white : Colors.grey[600],
                    ),
                  ),
                ),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: Text(
                  option.text,
                  style: TextStyle(
                    fontSize: 15,
                    color: isSelected ? AppTheme.chocolate : Colors.grey[800],
                  ),
                ),
              ),
              if (isSelected)
                const Icon(Icons.check_circle, color: AppTheme.caramel),
            ],
          ),
        ),
      ),
    );
  }
}

class _ResultScreen extends StatelessWidget {
  final PersonalityResult result;
  final String listingId;

  const _ResultScreen({required this.result, required this.listingId});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: SafeArea(
        child: SingleChildScrollView(
          padding: const EdgeInsets.all(24),
          child: Column(
            children: [
              const SizedBox(height: 40),
              
              // Confetti emoji
              const Text('🎉', style: TextStyle(fontSize: 64)),
              const SizedBox(height: 16),

              Text(
                'Your Cavoodle is a...',
                style: Theme.of(context).textTheme.titleLarge,
              ),
              const SizedBox(height: 8),

              // Personality type
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 12),
                decoration: BoxDecoration(
                  gradient: LinearGradient(
                    colors: [AppTheme.caramel, AppTheme.golden],
                  ),
                  borderRadius: BorderRadius.circular(30),
                ),
                child: Text(
                  result.personalityDisplayName,
                  style: const TextStyle(
                    fontSize: 24,
                    fontWeight: FontWeight.bold,
                    color: Colors.white,
                  ),
                  textAlign: TextAlign.center,
                ),
              ),
              const SizedBox(height: 24),

              // Bio
              if (result.customBio != null)
                Container(
                  padding: const EdgeInsets.all(16),
                  decoration: BoxDecoration(
                    color: AppTheme.pawPink.withOpacity(0.2),
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: Text(
                    '"${result.customBio}"',
                    style: Theme.of(context).textTheme.bodyLarge?.copyWith(
                      fontStyle: FontStyle.italic,
                    ),
                    textAlign: TextAlign.center,
                  ),
                ),
              const SizedBox(height: 32),

              // Stats
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                children: [
                  _StatCircle(
                    label: 'Energy',
                    value: result.energyLevel,
                    icon: '⚡',
                    color: Colors.orange,
                  ),
                  _StatCircle(
                    label: 'Floof',
                    value: result.floofFactor,
                    icon: '☁️',
                    color: Colors.blue,
                  ),
                  _StatCircle(
                    label: 'Sass',
                    value: result.sassRating,
                    icon: '😏',
                    color: Colors.purple,
                  ),
                ],
              ),
              const SizedBox(height: 40),

              // Actions
              SizedBox(
                width: double.infinity,
                child: ElevatedButton(
                  onPressed: () {
                    Navigator.of(context).popUntil((route) => route.isFirst);
                  },
                  child: const Text('Save & Continue'),
                ),
              ),
              const SizedBox(height: 12),
              TextButton(
                onPressed: () {
                  Navigator.of(context).pop();
                },
                child: const Text('Retake Quiz'),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _StatCircle extends StatelessWidget {
  final String label;
  final int value;
  final String icon;
  final Color color;

  const _StatCircle({
    required this.label,
    required this.value,
    required this.icon,
    required this.color,
  });

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Stack(
          alignment: Alignment.center,
          children: [
            SizedBox(
              width: 80,
              height: 80,
              child: CircularProgressIndicator(
                value: value / 5,
                strokeWidth: 8,
                backgroundColor: Colors.grey[200],
                valueColor: AlwaysStoppedAnimation<Color>(color),
              ),
            ),
            Text(icon, style: const TextStyle(fontSize: 28)),
          ],
        ),
        const SizedBox(height: 8),
        Text(
          label,
          style: const TextStyle(fontWeight: FontWeight.w600),
        ),
        Text(
          '$value/5',
          style: TextStyle(color: Colors.grey[600], fontSize: 12),
        ),
      ],
    );
  }
}
