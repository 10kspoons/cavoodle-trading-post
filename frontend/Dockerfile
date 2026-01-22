# 1. Build stage - Flutter SDK
FROM ghcr.io/cirruslabs/flutter:3.24.0 AS build
WORKDIR /app

# Copy pubspec first for caching
COPY pubspec.* ./
RUN flutter pub get

# Copy source and build
COPY . .
RUN flutter build web --release --web-renderer html

# 2. Final stage - Nginx
FROM nginx:1.25-alpine AS final

# Copy Flutter web build
COPY --from=build /app/build/web /usr/share/nginx/html

# Custom nginx config for SPA routing
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Security: Run as non-root
RUN adduser -D -u 1001 nginx_user && \
    chown -R nginx_user:nginx_user /usr/share/nginx/html && \
    chown -R nginx_user:nginx_user /var/cache/nginx && \
    chown -R nginx_user:nginx_user /var/log/nginx && \
    touch /var/run/nginx.pid && \
    chown nginx_user:nginx_user /var/run/nginx.pid

USER nginx_user

EXPOSE 8080
CMD ["nginx", "-g", "daemon off;"]
