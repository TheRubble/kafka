# Readme

## Course
https://github.com/conduktor/kafka-stack-docker-compose

## Warning(s)
Against all natural wants, do not use the async extensions if you care about the delivery order of the messages. 
This is due to the tasks completing on the threadpool and scheduling could be different - **A custom scheduler would have probably been better here?**

```
Another difference is that tasks returned by ProduceAsync compete on thread pool threads, so ordering is not guaranteed. By comparison, delivery report callbacks passed to Produce are all called on the same thread, so the ordering will exactly match the order they are known to be completed by the client.
```

## Topic guidelines

- Create twice as many partitions as brokers up until 12 partitions.
- Set compression - Helps with latency.
- Min.insync reps at least 2 maximum of 3.
- Replication factor of at least 3 at most 5.
- Carefully consider the key it'll impact the log compaction stuff later.

## Producer

- Idempotent
- Acks = All
- Consider linger to increase batch performance and the compression.
- Set retries etc, timeout is per operation it's across them all.

## Avro tools

```
dotnet tool install --global Apache.Avro.Tools
```

## Docker

Export the host IP :
```
$env:DOCKER_HOST_IP = 'host.docker.internal'
```

Shell into the container to the console:
```
docker exec -it kafka0 bash
```

## Create topic
```
kafka-topics --bootstrap-server localhost:9092 --create --topic customer.data.v1 --replication-factor 1 --partitions 3 --config compression.type=snappy
```

## Consume the topic in the console
```
kafka-console-consumer --bootstrap-server 127.0.0.1:9092 --topic customer.data.v1 --from-beginning --property print.value=true
```

## Delete topic
```
kafka-topics --bootstrap-server localhost:9092 --delete --topic customer.data.v1
```

## List all consumer groups
```
kafka-consumer-groups --list --bootstrap-server localhost:9092
```

## Reset the consumer group offset
```
kafka-consumer-groups --bootstrap-server 127.0.0.1:9092 --topic customer.data.v1 --to-earliest --group sample-api-group --reset-offsets --execute
```