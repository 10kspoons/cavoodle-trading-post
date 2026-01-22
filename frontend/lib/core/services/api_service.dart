import 'package:dio/dio.dart';
import '../config/api_config.dart';

class ApiService {
  static final ApiService _instance = ApiService._internal();
  factory ApiService() => _instance;
  
  late final Dio _dio;
  String? _authToken;

  ApiService._internal() {
    _dio = Dio(BaseOptions(
      baseUrl: ApiConfig.baseUrl,
      connectTimeout: const Duration(seconds: 10),
      receiveTimeout: const Duration(seconds: 10),
      headers: {
        'Content-Type': 'application/json',
      },
    ));

    _dio.interceptors.add(InterceptorsWrapper(
      onRequest: (options, handler) {
        if (_authToken != null) {
          options.headers['Authorization'] = 'Bearer $_authToken';
        }
        return handler.next(options);
      },
      onError: (error, handler) {
        // Handle 401 - redirect to login
        if (error.response?.statusCode == 401) {
          _authToken = null;
          // TODO: Navigate to login
        }
        return handler.next(error);
      },
    ));
  }

  void setAuthToken(String? token) {
    _authToken = token;
  }

  // Auth endpoints
  Future<AuthResponse> register(String email, String password, String displayName) async {
    final response = await _dio.post('/auth/register', data: {
      'email': email,
      'password': password,
      'displayName': displayName,
    });
    final authResponse = AuthResponse.fromJson(response.data);
    setAuthToken(authResponse.token);
    return authResponse;
  }

  Future<AuthResponse> login(String email, String password) async {
    final response = await _dio.post('/auth/login', data: {
      'email': email,
      'password': password,
    });
    final authResponse = AuthResponse.fromJson(response.data);
    setAuthToken(authResponse.token);
    return authResponse;
  }

  // Listings endpoints
  Future<ListingsResponse> getListings({
    String? color,
    String? state,
    double? minPrice,
    double? maxPrice,
    int page = 1,
    int pageSize = 20,
  }) async {
    final queryParams = <String, dynamic>{
      'page': page,
      'pageSize': pageSize,
    };
    if (color != null) queryParams['color'] = color;
    if (state != null) queryParams['state'] = state;
    if (minPrice != null) queryParams['minPrice'] = minPrice;
    if (maxPrice != null) queryParams['maxPrice'] = maxPrice;

    final response = await _dio.get('/listings', queryParameters: queryParams);
    return ListingsResponse.fromJson(response.data);
  }

  Future<ListingDetail> getListing(String id) async {
    final response = await _dio.get('/listings/$id');
    return ListingDetail.fromJson(response.data);
  }

  // Quiz endpoints
  Future<List<QuizQuestion>> getQuizQuestions() async {
    final response = await _dio.get('/quiz/questions');
    return (response.data as List)
        .map((q) => QuizQuestion.fromJson(q))
        .toList();
  }

  Future<PersonalityResult> calculatePersonality(Map<int, String> answers) async {
    final response = await _dio.post('/quiz/calculate', data: {
      'answers': answers.map((k, v) => MapEntry(k.toString(), v)),
    });
    return PersonalityResult.fromJson(response.data);
  }

  // User endpoints
  Future<UserProfile> getCurrentUser() async {
    final response = await _dio.get('/users/me');
    return UserProfile.fromJson(response.data);
  }
}

// Models
class AuthResponse {
  final String token;
  final String userId;
  final String displayName;
  final String email;

  AuthResponse({
    required this.token,
    required this.userId,
    required this.displayName,
    required this.email,
  });

  factory AuthResponse.fromJson(Map<String, dynamic> json) => AuthResponse(
    token: json['token'] ?? '',
    userId: json['userId'] ?? '',
    displayName: json['displayName'] ?? '',
    email: json['email'] ?? '',
  );
}

class ListingsResponse {
  final List<ListingSummary> items;
  final int totalCount;
  final int page;
  final int totalPages;

  ListingsResponse({
    required this.items,
    required this.totalCount,
    required this.page,
    required this.totalPages,
  });

  factory ListingsResponse.fromJson(Map<String, dynamic> json) => ListingsResponse(
    items: (json['items'] as List?)
        ?.map((i) => ListingSummary.fromJson(i))
        .toList() ?? [],
    totalCount: json['totalCount'] ?? 0,
    page: json['page'] ?? 1,
    totalPages: json['totalPages'] ?? 1,
  );
}

class ListingSummary {
  final String id;
  final String name;
  final int ageMonths;
  final String color;
  final String gender;
  final double price;
  final String suburb;
  final String state;
  final String? primaryPhotoUrl;
  final String? personalityType;
  final int floofFactor;
  final String? sellerName;
  final int sellerKarma;

  ListingSummary({
    required this.id,
    required this.name,
    required this.ageMonths,
    required this.color,
    required this.gender,
    required this.price,
    required this.suburb,
    required this.state,
    this.primaryPhotoUrl,
    this.personalityType,
    required this.floofFactor,
    this.sellerName,
    required this.sellerKarma,
  });

  factory ListingSummary.fromJson(Map<String, dynamic> json) => ListingSummary(
    id: json['id'] ?? '',
    name: json['name'] ?? '',
    ageMonths: json['ageMonths'] ?? 0,
    color: json['color'] ?? '',
    gender: json['gender'] ?? '',
    price: (json['price'] ?? 0).toDouble(),
    suburb: json['suburb'] ?? '',
    state: json['state'] ?? '',
    primaryPhotoUrl: json['primaryPhotoUrl'],
    personalityType: json['personalityType'],
    floofFactor: json['floofFactor'] ?? 0,
    sellerName: json['sellerName'],
    sellerKarma: json['sellerKarma'] ?? 0,
  );
}

class ListingDetail extends ListingSummary {
  final String description;
  final List<Photo> photos;
  final Personality? personality;
  final Seller seller;

  ListingDetail({
    required super.id,
    required super.name,
    required super.ageMonths,
    required super.color,
    required super.gender,
    required super.price,
    required super.suburb,
    required super.state,
    super.primaryPhotoUrl,
    super.personalityType,
    required super.floofFactor,
    required super.sellerKarma,
    required this.description,
    required this.photos,
    this.personality,
    required this.seller,
  });

  factory ListingDetail.fromJson(Map<String, dynamic> json) => ListingDetail(
    id: json['id'] ?? '',
    name: json['name'] ?? '',
    ageMonths: json['ageMonths'] ?? 0,
    color: json['color'] ?? '',
    gender: json['gender'] ?? '',
    price: (json['price'] ?? 0).toDouble(),
    suburb: json['suburb'] ?? '',
    state: json['state'] ?? '',
    primaryPhotoUrl: json['primaryPhotoUrl'],
    personalityType: json['personalityType'],
    floofFactor: json['floofFactor'] ?? 0,
    sellerKarma: (json['seller']?['cavoodleKarma'] ?? 0),
    description: json['description'] ?? '',
    photos: (json['photos'] as List?)
        ?.map((p) => Photo.fromJson(p))
        .toList() ?? [],
    personality: json['personality'] != null 
        ? Personality.fromJson(json['personality']) 
        : null,
    seller: Seller.fromJson(json['seller'] ?? {}),
  );
}

class Photo {
  final String url;
  final bool isPrimary;

  Photo({required this.url, required this.isPrimary});

  factory Photo.fromJson(Map<String, dynamic> json) => Photo(
    url: json['url'] ?? '',
    isPrimary: json['isPrimary'] ?? false,
  );
}

class Personality {
  final String type;
  final String typeDisplayName;
  final int energyLevel;
  final int floofFactor;
  final int sassRating;
  final String? customBio;

  Personality({
    required this.type,
    required this.typeDisplayName,
    required this.energyLevel,
    required this.floofFactor,
    required this.sassRating,
    this.customBio,
  });

  factory Personality.fromJson(Map<String, dynamic> json) => Personality(
    type: json['type'] ?? '',
    typeDisplayName: json['typeDisplayName'] ?? '',
    energyLevel: json['energyLevel'] ?? 0,
    floofFactor: json['floofFactor'] ?? 0,
    sassRating: json['sassRating'] ?? 0,
    customBio: json['customBio'],
  );
}

class Seller {
  final String id;
  final String displayName;
  final String? avatarUrl;
  final int cavoodleKarma;

  Seller({
    required this.id,
    required this.displayName,
    this.avatarUrl,
    required this.cavoodleKarma,
  });

  factory Seller.fromJson(Map<String, dynamic> json) => Seller(
    id: json['id'] ?? '',
    displayName: json['displayName'] ?? '',
    avatarUrl: json['avatarUrl'],
    cavoodleKarma: json['cavoodleKarma'] ?? 0,
  );
}

class QuizQuestion {
  final int id;
  final String question;
  final List<QuizOption> options;

  QuizQuestion({
    required this.id,
    required this.question,
    required this.options,
  });

  factory QuizQuestion.fromJson(Map<String, dynamic> json) => QuizQuestion(
    id: json['id'] ?? 0,
    question: json['question'] ?? '',
    options: (json['options'] as List?)
        ?.map((o) => QuizOption.fromJson(o))
        .toList() ?? [],
  );
}

class QuizOption {
  final String id;
  final String text;

  QuizOption({required this.id, required this.text});

  factory QuizOption.fromJson(Map<String, dynamic> json) => QuizOption(
    id: json['id'] ?? '',
    text: json['text'] ?? '',
  );
}

class PersonalityResult {
  final String personalityType;
  final String personalityDisplayName;
  final int energyLevel;
  final int floofFactor;
  final int sassRating;
  final String? customBio;

  PersonalityResult({
    required this.personalityType,
    required this.personalityDisplayName,
    required this.energyLevel,
    required this.floofFactor,
    required this.sassRating,
    this.customBio,
  });

  factory PersonalityResult.fromJson(Map<String, dynamic> json) => PersonalityResult(
    personalityType: json['personalityType'] ?? '',
    personalityDisplayName: json['personalityDisplayName'] ?? '',
    energyLevel: json['energyLevel'] ?? 0,
    floofFactor: json['floofFactor'] ?? 0,
    sassRating: json['sassRating'] ?? 0,
    customBio: json['customBio'],
  );
}

class UserProfile {
  final String id;
  final String email;
  final String displayName;
  final String? avatarUrl;
  final int cavoodleKarma;
  final List<String> badges;
  final int listingCount;
  final int reviewCount;

  UserProfile({
    required this.id,
    required this.email,
    required this.displayName,
    this.avatarUrl,
    required this.cavoodleKarma,
    required this.badges,
    required this.listingCount,
    required this.reviewCount,
  });

  factory UserProfile.fromJson(Map<String, dynamic> json) => UserProfile(
    id: json['id'] ?? '',
    email: json['email'] ?? '',
    displayName: json['displayName'] ?? '',
    avatarUrl: json['avatarUrl'],
    cavoodleKarma: json['cavoodleKarma'] ?? 0,
    badges: (json['badges'] as List?)?.cast<String>() ?? [],
    listingCount: json['listingCount'] ?? 0,
    reviewCount: json['reviewCount'] ?? 0,
  );
}
