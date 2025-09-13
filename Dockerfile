FROM oven/bun:alpine
LABEL authors="ev_ge"

WORKDIR /app

COPY bun.lock package.json ./

RUN bun install

COPY . .

CMD ["bun", "run", "./bot/bot.ts"]