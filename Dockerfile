FROM oven/bun:debian

# Устанавливаем зависимости для Chrome
RUN apt-get update && apt-get install -y \
    wget \
    gnupg \
    ca-certificates \
    fonts-liberation \
    libasound2 \
    libatk-bridge2.0-0 \
    libatk1.0-0 \
    libcups2 \
    libdbus-1-3 \
    libdrm2 \
    libgbm1 \
    libglib2.0-0 \
    libgtk-3-0 \
    libnspr4 \
    libnss3 \
    libu2f-udev \
    libvulkan1 \
    libxcomposite1 \
    libxdamage1 \
    libxfixes3 \
    libxrandr2 \
    libxkbcommon0 \
    libx11-xcb1 \
    libxext6 \
    libxshmfence1 \
    libpci3 \
    libxrender1 \
    xdg-utils \
    ffmpeg \
    && rm -rf /var/lib/apt/lists/*

# Рабочая директория
WORKDIR /app

# Dependencies
COPY bun.lock package.json ./
RUN bun install --frozen-lockfile

# Copy project
COPY . .

# Puppeteer будет использовать встроенный Chrome
ENV PUPPETEER_CACHE_DIR=/root/.cache/puppeteer
ENV PUPPETEER_SKIP_CHROMIUM_DOWNLOAD=false
ENV PUPPETEER_EXECUTABLE_PATH=/root/.cache/puppeteer/chrome/linux-135.0.7049.84/chrome-linux64/chrome
ENV PUPPETEER_ARGS="--no-sandbox --disable-setuid-sandbox"

CMD ["bun", "run", "./bot/bot.ts"]
