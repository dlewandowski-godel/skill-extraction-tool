FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
# Empty default: production builds use relative URLs (/api/...).
# Nginx proxies /api/ to the backend container (see nginx.conf).
# To bake in a different URL at image build time, pass:
#   docker build --build-arg VITE_API_URL=https://myapi.example.com .
ARG VITE_API_URL=
ENV VITE_API_URL=$VITE_API_URL
RUN npm run build

FROM nginx:alpine AS production
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 3000
CMD ["nginx", "-g", "daemon off;"]
