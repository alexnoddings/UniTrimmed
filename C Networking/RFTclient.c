#include "RFTheader.h"
#include <arpa/inet.h>
#include <errno.h>
#include <stdio.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <stdbool.h>
#include <time.h> 
#include <netdb.h> 
#include <string.h>

static const char* ERR_FMT = "%s:%d - %s, errno %d: %s\n";

int checksum(char* content, int len);
FILE* check_file(char* fileName);
int file_size(FILE* file);
void fatalerror(int line, char* msg);
bool isCorrupted(float prob);

struct sockaddr_in create_server_data(int sockfd, char* host, int port);
void send_meta_data(FILE* file, char* output, int sockfd, struct sockaddr_in server);
void send_file_normal(FILE* fp, int sockfd, struct sockaddr_in server);
void send_file_with_timeout(FILE* fp, int sockfd, struct sockaddr_in server, float prob_loss);

struct sockaddr_in create_server_data(int sockfd, char* host, int port)
{
	struct sockaddr_in server;
	struct hostent* lh;

	server.sin_family = AF_INET;
	server.sin_port = htons(port);

	lh = gethostbyname(host);
	if (!lh)
	{
		printf("SENDER: %s was not resolved\n", host);
		exit(1);
	}

	server.sin_addr.s_addr = inet_addr(host);

	return server;
}

void send_meta_data(FILE* file, char* output, int sockfd, struct sockaddr_in server)
{
	meta_data file_info;
	file_info.size = file_size(file);
	if (file_info.size < 1)
	{
		printf("SENDER: cannot send an empty file");
		exit(1);
	}

	int len = strlen(output);
	len = len >= sizeof(file_info.name) ? sizeof(file_info.name) - 1 : len;
	strncpy(file_info.name, output, len);
	file_info.name[len] = '\0';

	if (sendto(sockfd, &file_info, sizeof(file_info), 0, (struct sockaddr*)&server, sizeof(server)) <= 0)
	{
		perror("SENDER: sending metadata failed");
		exit(1);
	}
}

void send_file_normal(FILE* fp, int sockfd, struct sockaddr_in server)
{
	printf("SENDER: Start sending file\n");

	segment data_msg, ack_msg;
	data_msg.type = TYPE_DATA;
	int len = sizeof(server);
	int sq = 0;
	int payload_size = sizeof(data_msg.payload);

	fseek(fp, 0L, SEEK_SET);

	while (true)
	{
		data_msg.sq = sq;
		int size = fread(data_msg.payload, sizeof(char), payload_size, fp);
		data_msg.size = size;
		if (size < 15)
			data_msg.payload[size] = '\0';
		data_msg.checksum = checksum(data_msg.payload, size);

		printf("----------------------------------------------------\n");
		printf("SENDER: Sending segment: (sq:%d, size:%d, checksum:%d, content: '%.15s')\n", data_msg.sq, data_msg.size, data_msg.checksum, data_msg.payload);

		if (sendto(sockfd, &data_msg, sizeof(data_msg), 0, (struct sockaddr*)&server, len) < 0)
			perror("SENDER: error sending sequence\n");

		printf("SENDER: Waiting for an ack\n");

		if (recvfrom(sockfd, &ack_msg, sizeof(ack_msg), 0, (struct sockaddr*)&server, &len) < 0)
			perror("SENDER: error reading ack stream\n");

		if (ack_msg.sq != sq)
		{
			printf("SENDER: server's response ack does not match our ack (expected: %d, got: %d)", sq, ack_msg.sq);
			fclose(fp);
			close(sockfd);
			exit(1);
		}
		else
		{
			printf("SENDER: ACK sq=%d RECIEVED\n", ack_msg.sq);
		}

		sq = (sq + 1) % 2;

		if (size < 15) break;
	}
	printf("----------------------------------------------------\n");
}


void send_file_with_timeout(FILE* fp, int sockfd, struct sockaddr_in server, float prob_loss)
{
	printf("SENDER: Start sending file\n");

	segment data_msg, ack_msg;
	data_msg.type = TYPE_DATA;
	int len = sizeof(server);
	int val;
	int sq = 0;
	int payload_size = sizeof(data_msg.payload);
	int size;
	struct timeval timeout;
	timeout.tv_sec = 1;
	timeout.tv_usec = 0;
	fd_set recvfds;
	bool retrans = false;

	fseek(fp, 0L, SEEK_SET);

	while (true)
	{
		if (!retrans)
		{
			size = fread(data_msg.payload, sizeof(char), payload_size, fp);
			data_msg.sq = sq;
			data_msg.size = size;
			if (size < 15)
				data_msg.payload[size] = '\0';
		}
		retrans = false;

		printf("----------------------------------------------------\n");

		if (isCorrupted(prob_loss))
		{
			data_msg.checksum = 0;
			printf("SENDER: Simulating corrupted packet\n");
		}
		else
		{
			data_msg.checksum = checksum(data_msg.payload, size);
		}

		printf("SENDER: Sending segment: (sq:%d, size:%d, checksum:%d, content: '%.15s')\n", data_msg.sq, data_msg.size, data_msg.checksum, data_msg.payload);

		FD_ZERO(&recvfds);
		FD_SET(sockfd, &recvfds);
		if (sendto(sockfd, &data_msg, sizeof(data_msg), 0, (struct sockaddr*)&server, len) < 0)
		{
			perror("error sending sequence");
			fclose(fp);
			close(sockfd);
		}

		printf("SENDER: Waiting for an ack with timeout\n");
		int to = select(sockfd + 1, &recvfds, 0, 0, &timeout);
		if (to == -1)
		{
			perror("error waiting for ack in select");
			fclose(fp);
			close(sockfd);
			exit(1);
		}
		if (to == 0)
		{
			retrans = true;
			printf("SENDER: Waiting for ack timed out, retransmitting\n");
		}
		else
		{
			if (recvfrom(sockfd, &ack_msg, sizeof(ack_msg), 0, (struct sockaddr*)&server, &len) < 0)
			{
				perror("error reading ack stream");
				fclose(fp);
				close(sockfd);
				exit(1);
			}

			if (ack_msg.sq != sq)
			{
				printf("SENDER: server's response ack does not match our ack (expected: %d, got: %d)\n", sq, ack_msg.sq);
				fclose(fp);
				close(sockfd);
				exit(1);
			}
			else
			{
				printf("SENDER: ACK sq=%d RECIEVED\n", ack_msg.sq);
			}

			sq = (sq + 1) % 2;

			if (size < 15) break;
		}
	}
	printf("----------------------------------------------------\n");
}

void check_errno(char* msg)
{
	if (errno == 0) return;
	printf("\n\n\nerrno is %d. ", errno);
	printf("%s\n\n\n", msg);
}

int main(int argc, char* argv[])
{
	int sockfd;
	FILE* file;
	struct sockaddr_in server;

	if (argc != 5)
	{
		fprintf(stderr, "Usage: <inputFile> <outputFile> <localhost> <portNumber> \n");
		exit(1);
	}

	file = check_file(argv[1]);
	printf("----------------------------------------------------\n");
	printf("SENDER: File (%s) exists with a size of (%d) bytes\n", argv[1], file_size(file));
	printf("----------------------------------------------------\n");
	printf("----------------------------------------------------\n");

	sockfd = socket(AF_INET, SOCK_DGRAM, 0);
	if (sockfd < 0)
	{
		perror("SENDER: Failed to open socket: ");
		close(sockfd);
		exit(1);
	}
	printf("SENDER: Socket is created\n");

	server = create_server_data(sockfd, argv[3], atoi(argv[4]));
	printf("SENDER: Server data is prepared\n");

	send_meta_data(file, argv[2], sockfd, server);

	int choice = 0;
	float loss = 0;
	printf("----------------------------------------------------\n");
	printf("----------------------------------------------------\n");
	printf("Choose one of the following options (1 or 2):\n ");
	printf("1. Normal transmission (no data segment is lost) \n ");
	printf("2: Transmission with time-out capabilities\n ");
	scanf("%d", &choice);
	switch (choice)
	{
	case 1:
		send_file_normal(file, sockfd, server);
		break;
	case 2:
		printf("Enter the probability of a corrupted checksum (between 0 and 1):\n ");
		scanf("%f", &loss);
		send_file_with_timeout(file, sockfd, server, loss);
		break;
	default:
		printf("Error! enter 1 or 2 \n");
	}

	printf("SENDER: File is sent\n");

	fclose(file);
	close(sockfd);

	return 0;
}

int checksum(char* content, int len)
{
	int i;
	int sum = 0;
	for (i = 0; i < len; i++)
		sum += (int)(*content++);
	return sum;
}

FILE* check_file(char* fileName)
{
	FILE* file = fopen(fileName, "rb");
	if (!file) {
		perror("SENDER: File does not exists");
		fclose(file);
		exit(1);
	}
	return file;
}

int file_size(FILE* file)
{
	fseek(file, 0L, SEEK_END);
	int size = ftell(file);
	return size;
}

void fatalerror(int line, char* msg) {
	printf("Line %d : %s\n", line, msg);
	exit(0);
}

bool isCorrupted(float prob)
{
	srand(time(NULL));
	return ((float)(rand()) / (float)(RAND_MAX)) < prob;
}
