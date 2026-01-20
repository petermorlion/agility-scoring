# API Server

Express server with tRPC endpoints for the Agility Scoring mobile app.

## Development

Start the development server:

```bash
npm run dev
```

The server will run on `http://localhost:3000`

## Endpoints

### Health Check
- **GET** `/health` - Returns server status

### tRPC Endpoints
- **POST** `/trpc` - All tRPC endpoints are accessed through this endpoint

#### Available Procedures

- `hello` - Returns a greeting message with dummy user data
  - Input: `{ name?: string }`
  - Returns: greeting, timestamp, and user data

- `getDummyData` - Returns dummy competition data
  - Input: none
  - Returns: competitions, participants, and judges data

## Build

Build the TypeScript code:

```bash
npm run build
```

## Production

Run the built server:

```bash
npm start
```

## Tech Stack

- Express.js
- tRPC
- TypeScript
- Zod (validation)
