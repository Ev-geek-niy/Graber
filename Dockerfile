FROM oven/bun:debian
WORKDIR /app

COPY bun.lock package.json ./
RUN bun install --frozen-lockfile

COPY . .

CMD ["bun", "run", "./bot/bot.ts"]
