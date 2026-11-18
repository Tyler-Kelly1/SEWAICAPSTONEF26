# Stage 1: Build
FROM node:22-alpine AS build
WORKDIR /app

# Install dependencies with lockfile
COPY package*.json ./
RUN npm ci

# Copy frontend source
COPY . .

# Pass build-time API URL (default relative path for same-origin proxy)
ARG VITE_API_URL=/api/WorkoutSession
ENV VITE_API_URL=$VITE_API_URL

RUN npm run build

# Stage 2: Production Nginx Server
FROM nginx:alpine AS runtime

COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
