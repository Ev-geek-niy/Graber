# Base image
FROM oven/bun:alpine
LABEL authors="evgeekniy"

# Create working directory
WORKDIR /app

# Copy only dependency manifests first (для кеширования зависимостей)
COPY bun.lock package.json ./

# Install dependencies
RUN bun install --frozen-lockfile

# Copy the entire project
COPY . .

# Expose port if нужно (например, 3000)
# EXPOSE 3000

# Run bot
CMD ["bun", "run", "./bot/bot.ts"]
