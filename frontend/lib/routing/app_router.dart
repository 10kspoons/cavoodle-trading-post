import 'package:go_router/go_router.dart';
import '../features/home/home_screen.dart';
import '../features/listings/listing_detail_screen.dart';
import '../features/listings/create_listing_screen.dart';
import '../features/auth/login_screen.dart';
import '../features/auth/register_screen.dart';
import '../features/quiz/quiz_screen.dart';
import '../features/profile/profile_screen.dart';

final appRouter = GoRouter(
  initialLocation: '/',
  routes: [
    GoRoute(
      path: '/',
      name: 'home',
      builder: (context, state) => const HomeScreen(),
    ),
    GoRoute(
      path: '/listing/:id',
      name: 'listing',
      builder: (context, state) => ListingDetailScreen(
        listingId: state.pathParameters['id']!,
      ),
    ),
    GoRoute(
      path: '/create',
      name: 'create',
      builder: (context, state) => const CreateListingScreen(),
    ),
    GoRoute(
      path: '/login',
      name: 'login',
      builder: (context, state) => const LoginScreen(),
    ),
    GoRoute(
      path: '/register',
      name: 'register',
      builder: (context, state) => const RegisterScreen(),
    ),
    GoRoute(
      path: '/quiz/:listingId',
      name: 'quiz',
      builder: (context, state) => QuizScreen(
        listingId: state.pathParameters['listingId']!,
      ),
    ),
    GoRoute(
      path: '/profile',
      name: 'profile',
      builder: (context, state) => const ProfileScreen(),
    ),
  ],
);
