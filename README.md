# Chat Queue Management System

## Overview

The **Chat Queue Management System** is designed to manage and optimize the assignment of support requests (chats) to agents based on a variety of business rules. The system ensures efficient handling of chats, maintains proper agent workload, and provides robust monitoring capabilities.

## Features

- **FIFO Queue System**: Support requests are placed in a First-In-First-Out (FIFO) queue and monitored continuously.
- **Capacity Management**: The system calculates the maximum queue length and assigns chats to agents based on their capacity, which is determined by their seniority level.
- **Overflow Handling**: During office hours, when the main queue is full, an overflow team is activated to handle additional chats.
- **Agent Shift Management**: Agents work in 8-hour shifts. When their shift ends, they complete their current chats but are not assigned new ones.
- **Polling and Inactivity Monitoring**: The system polls every 1 second to check the status of chats and marks sessions inactive after missing 3 polls.
- **Round-Robin Chat Assignment**: Chats are assigned in a round-robin fashion, with preference given to agents with lower seniority to ensure higher-level agents are available to assist lower-level ones.

## Core Functionality

**Agent Assignment(RabbitMqConsumer.cs file) and TaskMonitor Services(MonitorChatSessionsTask.cs):**
The core functionalities, including the Agent assignment and TaskMonitor services, are implemented as extensions of IHostedService. This design choice ensures that these services are automatically managed by the .NET hosting environment. As IHostedService implementations, they start and stop automatically with the application, handling tasks like continuous monitoring and agent assignment without requiring manual intervention.
**Manual Agent Assignment API:**
Despite the automation provided by the hosted services, the API for agent assignment is also included. This API endpoint is available to support scenarios where manual intervention is needed. Sometimes support teams require the ability to manually assign chats to specific agents, particularly in cases where automated assignment may not align with immediate business needs. This flexibility ensures that the system can accommodate both automated and manual workflows.

## Business Logic

### 1. Initiating a Support Request

- Users initiate support requests through an API endpoint.
- The request is placed into a FIFO queue.

### 2. Queue and Overflow Management

- **Queue Monitoring**: The system monitors the queue to ensure that the maximum queue length is not exceeded.
- **Overflow Handling**: If the queue is full and it is during office hours, an overflow team is activated. If the overflow queue also becomes full, the chat is refused.

### 3. Chat Session Management

- **Polling**: Once a chat session is active, the chat window will poll the server every 1 second for updates.
- **Inactivity Detection**: If the server does not receive 3 consecutive polls, the session is marked as inactive.

### 4. Agent Assignment

- **Shift Management**: Agents work in 3 shifts, each 8 hours long. When an agent's shift ends, they finish their current chats but are not assigned new ones.
- **Capacity Calculation**: The maximum number of concurrent chats an agent can handle is calculated based on their seniority:
  - **Junior**: `0.4`
  - **Mid-Level**: `0.6`
  - **Senior**: `0.8`
  - **Team Lead**: `0.5`
- **Example Calculation**:
  - A team of 2 mid-levels and 1 junior has a capacity of:
    ```plaintext
    (2 x 10 x 0.6) + (1 x 10 x 0.4) = 16 concurrent chats capacity
    ```
  - The maximum queue length allowed is 24.

### 5. Chat Assignment Logic

- **Round-Robin Assignment**: Chats are assigned in a round-robin fashion:
  - Preference is given to the junior agents first, then mid-level, and finally senior agents.
  - This ensures that senior agents are more available to assist lower-level agents.

## Teams Available

- **Team A**: 1x Team Lead, 2x Mid-Level, 1x Junior
- **Team B**: 1x Senior, 1x Mid-Level, 2x Junior
- **Team C**: 2x Mid-Level (Night Shift Team)
- **Overflow Team**: 6x Junior (considered Junior for capacity calculations)

## Example Scenarios

### Scenario 1:

A team of 2 mid-levels and 1 junior:

- **Capacity Calculation**:
  ```plaintext
  (2 x 10 x 0.6) + (1 x 10 x 0.4) = 16 concurrent chats
  ```

## Business Logic

### Scenario 2: Chat Assignment

#### Team of 2 Agents:

- **1 Senior (Capacity 8)**, **1 Junior (Capacity 4)**.
- 5 chats arrive: 4 are assigned to the junior and 1 to the senior.

#### Team of 3 Agents:

- **2 Juniors**, **1 Mid-Level**.
- 6 chats arrive: 3 are assigned to each junior, none to the mid-level agent.

## Installation and Setup

### Clone the Repository

```bash
git clone https://github.com/yourusername/ChatQueueManagementSystem.git
cd ChatQueueManagementSystem
```

### Update Database with Migrations

The application automatically applies pending migrations and seeds the database on startup. Ensure your `appsettings.json` is configured with the correct database connection string.

### Run the Application

```bash
dotnet run --project ChatQueueManagementSystem.Presentation
```

## API Endpoints

- **Create Chat Session**: `POST /api/chat-sessions`
- **Assign Chat Session to Agent**: `POST /api/chat-sessions/{sessionId}/assign/{agentId}`
- **Get Chat Session Status**: `GET /api/chat-sessions/{sessionId}/status`

## Dependencies

- **.NET 8.0 SDK**
- **SQL Server**
- **Entity Framework Core**
- **MediatR**
- **AutoMapper**
- **FluentValidation**
- **Serilog**

## Application Implementation Details

The Chat Queue Management System has been designed with a clean architecture approach, separating concerns between different layers such as Presentation, Application, Domain, and Infrastructure.

### 1. Clean Architecture

The architecture follows the principles of Clean Architecture, where dependencies flow inward toward the Domain and Application layers. The outer layers (Presentation and Infrastructure) depend on the inner layers, but the inner layers are independent of the outer layers.

- **Domain Layer**: Contains the core business logic and domain entities such as Team, Agent, and ChatSession.
- **Application Layer**: Contains the business rules for handling operations such as creating a chat session, assigning chats to agents, and retrieving chat session status.
- **Infrastructure Layer**: Manages database interactions and external services like RabbitMQ for queuing.
- **Presentation Layer**: Exposes the application’s functionality through API endpoints, implemented using ASP.NET Core.

### 2. Repository Pattern

The repository pattern is used to abstract the database interactions. Repositories such as `ChatSessionRepository` and `AgentRepository` provide methods to perform CRUD operations and other complex queries.

However, this approach can lead to multiple database round trips, which might impact performance in high-load scenarios.

### 3. MediatR for CQRS

The application leverages MediatR to implement the Command Query Responsibility Segregation (CQRS) pattern. Each command or query is handled by a corresponding handler, ensuring a clear separation of concerns.

### 4. Validation

FluentValidation is used to validate incoming requests. This ensures that only valid data is processed by the application, reducing the likelihood of errors and exceptions.

### 5. Logging

Serilog is used for logging application events, which is essential for monitoring and troubleshooting.

## Recommendations for Future Enhancements

If given more time, I would implement the following enhancements:

### 1. Caching Mechanism

Implementing a caching mechanism would significantly reduce the load on the database by storing frequently accessed data in memory. This could be achieved using Redis or an in-memory cache like MemoryCache.

### 2. Rate Limiting

Adding rate limiting would prevent abuse of the API by limiting the number of requests a client can make in a given time period. This would protect the system from being overwhelmed by too many requests.

### 3. Load Balancing

Implementing load balancing would distribute incoming requests across multiple instances of the application, improving scalability and reliability. This can be done using tools like NGINX or Azure Load Balancer.

### 4. Performance Optimization

Further optimize the application's performance by reducing unnecessary round trips to the database. This could involve optimizing queries, reducing the use of the repository pattern in favor of direct service-based approaches, and ensuring that data access is as efficient as possible.

### 5. Raw Service Approach

Instead of using the repository pattern, I would consider using a raw service approach to interact with the database. This approach would allow for more direct control over database operations, reducing the overhead of multiple database round trips and improving overall performance.

## Endpoints Overview

### ChatSessionsController

- **POST** `/chat-sessions` - Create and queue a new chat session.
- **GET** `/chat-sessions/{sessionId}/status` - Poll for the status of a specific chat session.
- **POST** `/chat-sessions/{sessionId}/assign/{agentId}/agent` - Assign a chat session to a specific agent available.

### QueueManagementController

- **GET** `/chat-sessions/monitor` - Continuously monitor the chat session queue for inactivity or issues.
- **POST** `/queues/manage` - Manage the chat queue to handle overflow and ensure smooth operations.

### AgentsController

- **POST** `/agents/shifts/update` - Manage and update agent shifts to ensure proper coverage.

### TeamsController

- **GET** `/teams/capacity` - Retrieve information about team capacity and current chat loads.

## Detailed Explanation

The following features are essential to enhance the application's functionality and ensure optimal performance:

### ChatSessionsController

This controller is critical for managing the lifecycle of chat sessions. It allows for the creation and queuing of new chat sessions, tracking their status, and assigning them to available agents. By providing these operations, it ensures that customer interactions are handled efficiently and without delay.

### QueueManagementController

Effective queue management is vital to maintaining the responsiveness of the chat system. This controller offers the tools to monitor the chat queue for inactivity and manage overflow scenarios, preventing bottlenecks and ensuring that no customer is left waiting unnecessarily.

### AgentsController

The AgentsController is designed to manage agent shifts, ensuring that there is always adequate coverage for incoming chat sessions. Proper shift management helps in balancing agent workload and maintaining high service levels.

### TeamsController

Understanding team capacity and current chat loads is crucial for resource allocation and planning. This controller provides insights into how many sessions each team can handle, helping to optimize agent assignment and prevent overload.

## Additional Endpoints to be Implemented

To further enhance the application's functionality, the following endpoints are planned for future development:

- **POST** `/chat-sessions/{sessionId}/end` - End a specific chat session once it is completed.
- **PUT** `/queues/reorder` - Reorder the chat sessions in the queue based on priority or other criteria.
- **GET** `/agents/availability` - Check the availability of agents in real-time.
- **DELETE** `/teams/remove/{teamId}` - Remove a team from the system.

These additional endpoints will contribute to a more flexible and responsive chat management system, ensuring that the application can adapt to various scenarios and maintain high levels of customer satisfaction.
