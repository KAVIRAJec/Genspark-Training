# Stage 1: Build the Angular app
FROM node:20 AS build
WORKDIR /app
COPY package*.json ./
RUN npm install -g @angular/cli@latest
RUN npm install --force
COPY . .
RUN ng build --configuration=production

# Stage 2: Serve the app with NGINX
FROM nginx:alpine
COPY --from=build /app/dist/Freelance-Project/browser /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]